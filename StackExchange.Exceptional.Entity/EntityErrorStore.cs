using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StackExchange.Exceptional.Entity
{
    public class EntityErrorStore : ErrorStore
    {
        public static IPersistenceProvider PersistanceProvider { get; set; }

        public EntityErrorStore(ErrorStoreSettings settings) : base(settings) { }

        public EntityErrorStore(int rollupSeconds, int backupQueueSize = 1000) : base(rollupSeconds, backupQueueSize) { }

        protected override bool DeleteAllErrors(string applicationName = null)
        {
            if (!IsConfigured()) throw new NotImplementedException();
            return PersistanceProvider.DeleteAllErrors(applicationName);
        }

        protected override bool DeleteError(Guid guid)
        {
            if (!IsConfigured()) throw new NotImplementedException();
            return PersistanceProvider.DeleteError(guid);
        }

        protected override int GetAllErrors(List<Error> list, string applicationName = null)
        {
            if (!IsConfigured()) throw new NotImplementedException();
            var sourceList = new List<IPersistedError>();
            int retval = PersistanceProvider.GetAllErrors(sourceList, applicationName);
            list.AddRange(sourceList.Select(ConvertToError));
            return retval;
        }

        protected override Error GetError(Guid guid)
        {
            if (!IsConfigured()) throw new NotImplementedException();
            return ConvertToError(PersistanceProvider.GetError(guid));
        }

        protected override int GetErrorCount(DateTime? since = default(DateTime?), string applicationName = null)
        {
            if (!IsConfigured()) throw new NotImplementedException();
            return PersistanceProvider.GetErrorCount(since, applicationName);
        }

        protected override void LogError(Error error)
        {
            if (!IsConfigured()) throw new NotImplementedException();
            var retval = PersistanceProvider.LogError(error);
            if (retval != null && retval.GUID != error.GUID) error.IsDuplicate = true;
        }

        protected override bool ProtectError(Guid guid)
        {
            if (!IsConfigured()) throw new NotImplementedException();
            return PersistanceProvider.ProtectError(guid);
        }

        protected Error ConvertToError(IPersistedError source)
        {
            var e = Error.FromJson(source.FullJson);
            e.DuplicateCount = source.DuplicateCount;
            //LastLogDate will be added in StackExchange.Exceptional v2
            //e.LastLogDate = source.LastLogDate;
            e.DeletionDate = source.DeletionDate;
            e.IsProtected = source.IsProtected;
            return e;
        }

        private bool _AttemptedConfig = false;
        protected bool IsConfigured()
        {
            if (PersistanceProvider != null) return true;
            if (_AttemptedConfig) return false;
            _AttemptedConfig = true;
            var assm = System.Reflection.Assembly.GetExecutingAssembly();
            var ti = typeof(IPersistenceProvider);
            var implTypes = assm.GetTypes().Where(x => x.IsClass && !x.IsAbstract && x.GetInterface(ti.Name) == ti);
            var match = implTypes.FirstOrDefault(x => x.GetConstructor(Type.EmptyTypes) != null);
            if (match == null) return false;
            PersistanceProvider = Activator.CreateInstance(match, true) as IPersistenceProvider;
            return PersistanceProvider != null;
        }
    }
}
