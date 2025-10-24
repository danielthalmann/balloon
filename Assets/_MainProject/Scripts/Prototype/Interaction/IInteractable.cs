using UnityEngine;

namespace Prototype.Interaction
{
    public interface IInteractable
    {
        void OnClicked();
        string GetDescription();
    }
}
