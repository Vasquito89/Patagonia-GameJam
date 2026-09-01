using UnityEngine;

namespace Goru.Core
{
    // Abstracción para los datos de entrada del jugador
    public interface IInputProvider
    {
        Vector2 MoveInput { get; }
        Vector2 LookInput { get; }
        bool JumpRequested { get; }
        bool SprintRequested { get; }
        bool BaldeRequested { get; }
        bool SecarRequested { get; }
        void ConsumeJump();
        void ConsumeBalde();
        void ConsumeSecar();
    }
}
