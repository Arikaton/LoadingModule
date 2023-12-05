using Cysharp.Threading.Tasks;
using LoadingModule.Contracts;

namespace LoadingModule.Entity
{
    /// <summary>
    /// Helper class for loading steps, which should not provide any artifacts
    /// </summary>
    public abstract class EmptyLoadingStep : LoadingStep
    {
        protected EmptyLoadingStep() : base(typeof(EmptyArtifact))
        {
        }

        protected abstract UniTask Execute();

        protected override async UniTask<ILoadingArtifact> Load()
        {
            await Execute();
            return new EmptyArtifact();
        }
    }
}