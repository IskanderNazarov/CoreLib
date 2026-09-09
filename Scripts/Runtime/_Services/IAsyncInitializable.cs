using Cysharp.Threading.Tasks;

namespace Core._Services {
    public interface IAsyncInitializable {
        UniTask Initialize();
    }
}
