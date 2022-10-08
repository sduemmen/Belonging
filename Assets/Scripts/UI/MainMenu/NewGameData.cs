using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

namespace UI.MainMenu
{
    public class NewGameData : MonoBehaviour
    {
        public TMP_InputField nameField;
        public TMP_InputField seedField;
        public Button generateSeedButton;
        public Slider treeQuantitySlider;
        public TextMeshProUGUI treeQuantityLabel;
        public Slider stoneQuantitySlider;
        public TextMeshProUGUI stoneQuantityLabel;
        public Toggle unlockAllToggle;
        public RawImage previewImage;

        public string gameName = "New World";
        public int seed;
        public float treeThreshold = .4f;
        public float stoneThreshold = .3f;
        public bool unlockAll;
        
        public float persistance = .4f;
        public int roughness = 3;
        public int octaves = 3;

        private void OnValidate()
        {
            GeneratePreviewImage();
        }

        private void Awake()
        {
            nameField.contentType = TMP_InputField.ContentType.Standard;
            nameField.onValueChanged.AddListener(value => gameName = value);
            
            seed = new Random().Next(100000, 100000000);
            seedField.text = seed.ToString();
            seedField.contentType = TMP_InputField.ContentType.IntegerNumber;
            seedField.onValueChanged.AddListener(value => seed = Int32.Parse(value));
            
            generateSeedButton.onClick.AddListener(GenerateNewSeed);
            
            treeQuantitySlider.minValue = .2f;
            treeQuantitySlider.maxValue = .8f;
            treeQuantitySlider.onValueChanged.AddListener(value => {
                treeThreshold = value;
                treeQuantityLabel.SetText(value.ToString("P"));
                GeneratePreviewImage();
            });
            treeQuantitySlider.value = .4f;
            
            stoneQuantitySlider.minValue = .2f;
            stoneQuantitySlider.maxValue = .8f;
            stoneQuantitySlider.onValueChanged.AddListener(value => {
                stoneThreshold = value;
                stoneQuantityLabel.SetText(value.ToString("P"));
                GeneratePreviewImage();
            });
            stoneQuantitySlider.value = .3f;
            
            unlockAllToggle.onValueChanged.AddListener(value => unlockAll = value);
        }

        private void OnDestroy()
        {
            nameField.onValueChanged.RemoveAllListeners();
            seedField.onValueChanged.RemoveAllListeners();
            generateSeedButton.onClick.RemoveAllListeners();
            treeQuantitySlider.onValueChanged.RemoveAllListeners();
            stoneQuantitySlider.onValueChanged.RemoveAllListeners();
            unlockAllToggle.onValueChanged.RemoveAllListeners();
        }

        public void GenerateNewSeed()
        {
            int randomSeed = new Random().Next(100000, 100000000);
            seed = randomSeed;
            seedField.text = randomSeed.ToString();
            GeneratePreviewImage();
        }

        public void GeneratePreviewImage()
        {
            if (seed == 0) GenerateNewSeed();
            var rect = previewImage.rectTransform.rect;
            int width = (int)rect.width;
            int height = (int)rect.height;
            
            float treeQuantity = 1 - this.treeThreshold;
            float stoneQuantity = this.stoneThreshold;

            Texture2D texture2D = new Texture2D(width, height) {
                wrapMode = TextureWrapMode.Clamp
            };

            previewImage.texture = texture2D;

            Random random = new Random();
            
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    float seededX = x + (float)seed / 100;
                    float seededY = y + (float)seed / 100;
                    
                    float treeSample = CalculateNoise(seededX, seededY);
                    float stoneSample = CalculateNoise(seededX + 50f, seededY + 50f);
                    
                    bool decider = false;
                    bool _override = false;
                    if (treeSample > treeQuantity && stoneSample < stoneQuantity) {
                        decider = Convert.ToBoolean(random.Next(0, 2));
                        _override = true;
                    } 
                    
                    if (treeSample > treeQuantity && !_override || _override && decider) {
                        texture2D.SetPixel(x, y, new Color(.23f, .45f, .28f, 1));
                    } else if (stoneSample < stoneQuantity && !_override || _override) {
                        texture2D.SetPixel(x, y, Color.gray);
                    } else {
                        texture2D.SetPixel(x, y, Color.white);
                    }
                }
            }
            
            texture2D.Apply();
            
            float CalculateNoise(float x, float y)
            {
                float xCoord = x / width * 5;
                float yCoord = y / height * 5;
                
                float noise = 0;
                float frequency = 1;
                float factor = 1;

                for (int i = 0; i < octaves; i++) {
                    noise += Mathf.PerlinNoise(xCoord * frequency + i, yCoord * frequency + i) * factor;
                    factor *= persistance;
                    frequency *= roughness;
                }
                
                return noise -.25f;
            }
        }
    }
}