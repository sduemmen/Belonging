using Environment;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Collections
{
    [CreateAssetMenu(menuName = "Collections/Segment Collection")]
    public class SegmentCollection : Collection<SegmentCollectionEntry>
    {
        protected override void VerifyEntries()
        {
            base.VerifyEntries();
            
            for (int i = 0; i < entries.Count; i++) {
                if (entries[i].prefab == null) {
                    Debug.LogWarning($"prefab not assigned at index {i}");
                }
                
                if (string.IsNullOrEmpty(entries[i].prefabName)) {
                    Debug.LogWarning($"prefabName not assigned at index {i}");
                }

                if (entries[i].previewImage == null) {
                    Debug.LogWarning($"previewImage not assigned at index {i}");
                }

                if (entries[i].buildCosts.Count <= 0) {
                    Debug.LogWarning($"no build costs assigned at index {i}");
                }

                if (string.IsNullOrEmpty(entries[i].displayName)) {
                    Debug.LogWarning($"no display name assigned at index {i}");
                }

                if (entries[i].prefab.GetComponent<Destroyable>().itemDrops != entries[i].buildCosts) {
                    Debug.Log($"build cost out of sync with item drops at index {i}");
                }
            }
        }

        [Button("Fix Problems")]
        private void FixProblems() {
            
            for (int i = 0; i < entries.Count; i++) {
                if (string.IsNullOrEmpty(entries[i].prefabName)) {
                    if (entries[i].prefab != null) {
                        entries[i].prefabName = entries[i].prefab.name;
                        Debug.Log($"set prefabName at index {i}");
                    }
                } else {
                    if (entries[i].prefab != null && entries[i].prefabName != entries[i].prefab.name) {
                        entries[i].prefabName = entries[i].prefab.name;
                        Debug.Log($"fixed prefabName at index {i}");
                    }
                }

                if (entries[i].prefab.GetComponent<Destroyable>().itemDrops != entries[i].buildCosts) {
                    entries[i].prefab.GetComponent<Destroyable>().itemDrops = entries[i].buildCosts;
                    Debug.Log($"fixed build cost out of sync with item drops at index {i}");
                }
            }
        }
    }
}