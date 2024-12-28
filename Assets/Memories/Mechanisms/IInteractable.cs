﻿using Memories.Characters;

namespace Memories.Mechanisms
{
    public interface IInteractable
    {
        bool CanInteract(Player player);
        void Interact(Player player);
        void DiscardAfterUse(Player player) { }
    }
}
