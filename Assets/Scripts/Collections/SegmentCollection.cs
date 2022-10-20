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
                    if (entries[i].prefab != null) {
                        entries[i].prefabName = entries[i].prefab.name;
                        Debug.Log($"set prefabName at index {i}");
                    } else {
                        Debug.LogWarning($"prefabName not assigned at index {i}");
                    }
                } else {
                    if (entries[i].prefab != null && entries[i].prefabName != entries[i].prefab.name) {
                        entries[i].prefabName = entries[i].prefab.name;
                        Debug.Log($"fixed prefabName at index {i}");
                    }
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
            }
        }
    }
}