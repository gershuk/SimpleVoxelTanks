using System;
using System.Collections.Generic;

using SimpleVoxelTanks.MapBuilders;

using UnityEngine;

namespace SimpleVoxelTanks.LevelControllers
{
    public abstract class AbstractLevelScript : MonoBehaviour
    {
        public event Action OnLose;

        public event Action OnWin;

        protected Type[] _aiTypes;

        public IReadOnlyList<Type> AiTypes => _aiTypes;

        public AbstractMapBuilder MapBuilder { get; protected set; }

        protected void RiseLoseEvent () => OnLose?.Invoke();

        protected void RiseWinEvent () => OnWin?.Invoke();

        public virtual void Init (AbstractMapBuilder abstractMapBuilder) => MapBuilder = abstractMapBuilder;

        public abstract void StartScript ();
    }
}