using Environment;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Collections
{
    [CreateAssetMenu(menuName = "Collections/Segment Collection")]
    public class SegmentCollection : Collection<SegmentCollectionEntry>
    {
#if UNITY_EDITOR
        protected override void VerifyEntries()
        {
            base.VerifyEntries();
            
            for (int i = 0; i < entries.Count; i++) {
                if (entries[i].prefab == null) {
                    Debug.LogWarning($"prefab not assigned in {entries[i].prefabName}");
                }
                
                if (string.IsNullOrEmpty(entries[i].prefabName)) {
                    Debug.LogWarning($"prefabName not assigned in {entries[i].prefabName}");
                }

                if (entries[i].previewImage == null) {
                    Debug.LogWarning($"previewImage not assigned in {entries[i].prefabName}");
                }

                if (entries[i].buildCosts.Count <= 0) {
                    Debug.LogWarning($"no build costs assigned in {entries[i].prefabName}");
                }

                if (string.IsNullOrEmpty(entries[i].displayName)) {
                    Debug.LogWarning($"no display name assigned in {entries[i].prefabName}");
                }

                for (int j = 0; j < entries[i].buildCosts.Count; j++) {
                    if (j < entries[i].prefab.GetComponent<Destroyable>().itemDrops.Count && !Equals(entries[i].prefab.GetComponent<Destroyable>().itemDrops[j], entries[i].buildCosts[j])) {
                        Debug.Log($"build cost {entries[i].buildCosts[j].item.displayName} out of sync with item drops in {entries[i].prefabName}");
                    } else if (entries[i].prefab.GetComponent<Destroyable>().itemDrops.Count != entries[i].buildCosts.Count) {
                        Debug.Log($"build cost list out of sync with item drops in {entries[i].prefabName}");
                    }
                }
            }
        }

        [Button("Fix Problems")]
        private void FixProblems() {
            
            for (int i = 0; i < entries.Count; i++) {
                if (string.IsNullOrEmpty(entries[i].prefabName)) {
                    if (entries[i].prefab != null) {
                        entries[i].prefabName = entries[i].prefab.name;
                        Debug.Log($"set prefabName in {entries[i].prefabName}");
                    }
                } else {
                    if (entries[i].prefab != null && entries[i].prefabName != entries[i].prefab.name) {
                        entries[i].prefabName = entries[i].prefab.name;
                        Debug.Log($"fixed prefabName in {entries[i].prefabName}");
                    }
                }

                if (entries[i].prefab.GetComponent<Destroyable>().itemDrops != entries[i].buildCosts) {
                    entries[i].prefab.GetComponent<Destroyable>().itemDrops = entries[i].buildCosts;
                    EditorUtility.SetDirty(entries[i].prefab);
                    Debug.Log($"fixed build cost out of sync with item drops in {entries[i].prefabName}");
                }
            }
        }
#endif
    }
}