using System;
using Grpc.Core;

namespace Meeting.Core.GrpcClient
{
    internal sealed class MetadataRepository
    {
        public Metadata CurrentMetadata { get; private set; }

        public event EventHandler<Metadata> MetadataChanged;

        public void SetMetadata(Metadata metadata)
        {
            if (CurrentMetadata == metadata)
                return;

            CurrentMetadata = metadata;
            MetadataChanged?.Invoke(this, metadata);
        }
    }
}
