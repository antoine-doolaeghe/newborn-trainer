#if INVENTORYPRODEVDOG
using Devdog.InventoryPro;
using EasyBuildSystem.Runtimes.Events;
using EasyBuildSystem.Runtimes.Internal.Builder;
using EasyBuildSystem.Runtimes.Internal.Part;
using EasyBuildSystem.Runtimes.Internal.Socket;
using UnityEngine;

public class EbsCustomItemType : InventoryItemBase
{
    #region Public Fields

    public PartBehaviour Part;

    public bool UpgradeTool;
    public bool DestroyTool;

    #endregion Public Fields

    #region Private Fields

    private PartBehaviour CurrentPrefab;

    #endregion Private Fields

    #region Public Methods

    public override int Use()
    {
        int Used = base.Use();

        EventHandlers.OnPlacedPart -= OnBuildPrefabPlaced;
        EventHandlers.OnDestroyedPart -= OnBuildDestroyed;

        if (Used < 0)
            return Used;

        EventHandlers.OnPlacedPart += OnBuildPrefabPlaced;
        EventHandlers.OnDestroyedPart += OnBuildDestroyed;

        if (DestroyTool)
            BuilderBehaviour.Instance.ChangeMode(BuildMode.Destruction);
        else if (UpgradeTool)
        {
            PartBehaviour Target = BuilderBehaviour.Instance.GetTargetedPart();

            if (Target != null)
            {
                int Up = Target.AppearanceIndex + 1;

                if (Up <= Target.Appearances.Count-1)
                    Target.ChangeAppearance(Up);
            }
        }
        else
        {
            BuilderBehaviour.Instance.SelectPrefab(Part);

            BuilderBehaviour.Instance.ChangeMode(BuildMode.Placement);

            CurrentPrefab = Part;
        }

        return 0;
    }

    #endregion Public Methods

    #region Private Methods

    private void OnBuildPrefabPlaced(PartBehaviour part, SocketBehaviour socket)
    {
        if (CurrentPrefab == null)
            return;

        if (CurrentPrefab.Id != part.Id)
            return;

        base.Use();

        currentStackSize--;

        NotifyItemUsed(1, true);

        BuilderBehaviour.Instance.ChangeMode(BuildMode.None);
    }

    private void OnBuildDestroyed(PartBehaviour prefab)
    {
        base.Use();

        NotifyItemUsed(1, true);
    }

    #endregion Private Methods
}
#endif