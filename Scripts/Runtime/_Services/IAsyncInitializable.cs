using Cysharp.Threading.Tasks;

namespace _Services {
    public interface IAsyncInitializable {
        UniTask Initialize();
    }
}
