using Neo.IO;
using Neo.IO.Caching;
using Neo.Ledger;
using Neo.Network.P2P.Payloads;
using Neo.Persistence;

namespace Neo.TestingEngine
{
    public class TestSnapshot : StoreView
    {
        private DataCache<UInt256, TrimmedBlock> _Blocks = new TestDataCache<UInt256, TrimmedBlock>();
        private DataCache<UInt256, TransactionState> _Transactions = new TestDataCache<UInt256, TransactionState>();
        private DataCache<UInt160, ContractState> _Contracts = new TestDataCache<UInt160, ContractState>();
        private DataCache<StorageKey, StorageItem> _Storages = new TestDataCache<StorageKey, StorageItem>();
        private DataCache<SerializableWrapper<uint>, HeaderHashList> _HeaderHashList = new TestDataCache<SerializableWrapper<uint>, HeaderHashList>();
        private MetaDataCache<HashIndexState> _BlockHashIndex = new TestMetaDataCache<HashIndexState>();
        private MetaDataCache<HashIndexState> _HeaderHashIndex = new TestMetaDataCache<HashIndexState>();
        private MetaDataCache<ContractIdState> _ContractId = new TestMetaDataCache<ContractIdState>();

        public override DataCache<UInt256, TrimmedBlock> Blocks => _Blocks;

        public override DataCache<UInt256, TransactionState> Transactions => _Transactions;

        public override DataCache<UInt160, ContractState> Contracts => _Contracts;

        public override DataCache<StorageKey, StorageItem> Storages => _Storages;

        public override MetaDataCache<ContractIdState> ContractId => _ContractId;

        public override DataCache<SerializableWrapper<uint>, HeaderHashList> HeaderHashList => _HeaderHashList;

        public override MetaDataCache<HashIndexState> BlockHashIndex => _BlockHashIndex;

        public override MetaDataCache<HashIndexState> HeaderHashIndex => _HeaderHashIndex;

        /// <summary>
        /// Set Persisting block for unit test
        /// </summary>
        /// <param name="block">Block</param>
        public void SetPersistingBlock(Block block)
        {
            this.GetType().GetProperty("PersistingBlock").SetValue(this, block);
        }

        /// <summary>
        /// Clear the storage for unit test
        /// </summary>
        public void ClearStorage()
        {
            ((TestDataCache<StorageKey, StorageItem>)this._Storages).Clear();
        }

        public void SetCurrentBlockHash(uint index, UInt256 hash)
        {
            if (hash != null && Blocks is TestDataCache<UInt256, TrimmedBlock> blocks)
            {
                var blocksCount = blocks.Count();

                if (index > blocksCount)
                {
                    index = (uint)blocksCount;
                }

                var hashIndex = new HashIndexState()
                {
                    Hash = hash,
                    Index = index
                };
                ((TestMetaDataCache<HashIndexState>)BlockHashIndex).Update(hashIndex);
            }
        }
    }
}
