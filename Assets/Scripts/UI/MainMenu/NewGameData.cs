using System;
using System.Collections.Generic;
using Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using World;
using Random = System.Random;

namespace UI.MainMenu
{
    public class NewGameData : MonoBehaviour
    {
        public TMP_InputField nameField;
        public TMP_InputField seedField;
        public Button generateSeedButton;
        public Slider treeDensitySlider;
        public TextMeshProUGUI treeDensityLabel;
        public Slider stoneDensitySlider;
        public TextMeshProUGUI stoneDensityLabel;
        public Toggle unlockAllToggle;
        public RawImage previewImage;
        private int _previewImageWidth;
        private int _previewImageHeight;

        public string gameName = "New World";
        public Vector3 playerSpawnPosition = Vector3.zero;
        public int seed;
        public float treeDensityThreshold = .4f;
        public float stoneDensityThreshold = .4f;
        public bool unlockAll;
        
        [SerializeField] private Color _stoneColor = Color.gray;
        [SerializeField] private Color _treeColor = new Color(.23f, .45f, .28f, 1);
        [SerializeField] private Color _backgroundColor = new Color(.1f, .1f, .1f, .5f);
        [SerializeField] private Color _playerSpawnPointColor = new Color(.8f, .4f, .4f);

        private void Awake()
        {
            nameField.contentType = TMP_InputField.ContentType.Standard;
            nameField.onValueChanged.AddListener(value => gameName = value);

            seed = new Random().Next(100000, 100000000);
            seedField.text = seed.ToString();
            seedField.contentType = TMP_InputField.ContentType.IntegerNumber;
            seedField.onValueChanged.AddListener(value => seed = int.Parse(value));

            generateSeedButton.onClick.AddListener(GenerateNewSeed);

            treeDensitySlider.minValue = .2f;
            treeDensitySlider.maxValue = .8f;
            treeDensitySlider.onValueChanged.AddListener(value => {
                if ((treeDensityThreshold < .4f && value >= .4f) || (treeDensityThreshold > .4f && value <= .4f) ||
                    (treeDensityThreshold < .6f && value >= .6f) || (treeDensityThreshold > .6f && value <= .6f))
                {
                    AudioController.Instance.PlayAudio("UIHoverSound");
                }
                
                treeDensityThreshold = value;
                string text = "";
                if (value < .4f)
                    text = "Low";
                else if (value < .6f)
                    text = "Medium";
                else
                    text = "High";
                treeDensityLabel.SetText(text);
                GeneratePreviewImage();
            });
            treeDensitySlider.value = treeDensityThreshold;

            stoneDensitySlider.minValue = .2f;
            stoneDensitySlider.maxValue = .8f;
            stoneDensitySlider.onValueChanged.AddListener(value => {
                if ((stoneDensityThreshold < .4f && value >= .4f) || (stoneDensityThreshold > .4f && value <= .4f) ||
                    (stoneDensityThreshold < .6f && value >= .6f) || (stoneDensityThreshold > .6f && value <= .6f))
                {
                    AudioController.Instance.PlayAudio("UIHoverSound");
                }
                
                stoneDensityThreshold = value;
                string text = "";
                if (value < .4f)
                    text = "Low";
                else if (value < .6f)
                    text = "Medium";
                else
                    text = "High";
                stoneDensityLabel.SetText(text);
                GeneratePreviewImage();
            });
            stoneDensitySlider.value = stoneDensityThreshold;

            unlockAllToggle.onValueChanged.AddListener(value => unlockAll = value);

            Rect rect = previewImage.rectTransform.rect;

            _previewImageWidth = (int)rect.width;
            _previewImageHeight = (int)rect.height;
            
            if (previewImage.texture == null)
            {
                Texture2D texture2D = new Texture2D(_previewImageWidth, _previewImageHeight) {
                    wrapMode = TextureWrapMode.Clamp,
                    filterMode = FilterMode.Point,
                };

                previewImage.texture = texture2D;
            }
        }

        private void OnDestroy()
        {
            nameField.onValueChanged.RemoveAllListeners();
            seedField.onValueChanged.RemoveAllListeners();
            generateSeedButton.onClick.RemoveAllListeners();
            treeDensitySlider.onValueChanged.RemoveAllListeners();
            stoneDensitySlider.onValueChanged.RemoveAllListeners();
            unlockAllToggle.onValueChanged.RemoveAllListeners();
        }

        private void OnValidate()
        {
            GeneratePreviewImage();
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
            if (seed == 0)
            {
                GenerateNewSeed();
            }
            
            int width = _previewImageWidth / 2;
            int height = _previewImageHeight / 2;

            Texture2D previewTexture = (Texture2D)previewImage.texture;

            Random random = new Random(seed);
            float xOffset = random.Next(-10000, 10000);
            float yOffset = random.Next(-10000, 10000);
            float[,] treeSamples = new float[width, height];
            float[,] stoneSamples = new float[width, height];

            // draw stones and trees to preview image
            for (int py = 0; py < height; py++)
            {
                for (int px = 0; px < width; px++)
                {
                    float treeSample = WorldGenerator.SamplePerlin2d(px, py, xOffset, yOffset);
                    float stoneSample = WorldGenerator.SamplePerlin2d(px + 1000, py + 1000, xOffset, yOffset);
                    treeSamples[px, py] = treeSample;
                    stoneSamples[px, py] = stoneSample;

                    if (treeSample < treeDensityThreshold && stoneSample < stoneDensityThreshold)
                    {
                        bool decider = Convert.ToBoolean(random.Next(0, 2));
                        Color color = decider ? _stoneColor : _treeColor;
                        previewTexture.SetPixel(px, py, color);
                        continue;
                    }

                    if (treeSample < treeDensityThreshold)
                    {
                        previewTexture.SetPixel(px, py, _treeColor);
                    }
                    else if (stoneSample < stoneDensityThreshold)
                    {
                        previewTexture.SetPixel(px, py, _stoneColor);
                    }
                    else
                    {
                        previewTexture.SetPixel(px, py, _backgroundColor);
                    }
                }
            }

            // find spawn position for player
            bool spawnFound = false;
            var visited = new Dictionary<Vector2Int, bool>();
            Vector2Int[] turns = {
                new(0, 1),
                new(1, 0),
                new(0, -1),
                new(-1, 0)
            };

            int x = width / 2;
            int y = height / 2;
            int currentTurnIndex = 0;

            while (!spawnFound)
            {
                // check for out of bounds
                if (x <= 0 || x >= width || y <= 0 || y >= height)
                {
                    Debug.Log("Unable to set spawn position");
                    break;
                }

                // evaluate samples
                float treeSample = treeSamples[x, y];
                float stoneSample = stoneSamples[x, y];

                if (treeSample - .1f > treeDensityThreshold && stoneSample - .1f > stoneDensityThreshold)
                {
                    playerSpawnPosition = new Vector3(x, 0, y);
                    spawnFound = true;
                }

                visited.Add(new Vector2Int(x, y), true);

                // update x, y, currentTurnIndex
                currentTurnIndex %= 4;
                int nextTurnIndex = (currentTurnIndex + 1) % 4;
                if (!visited.ContainsKey(new Vector2Int(x, y) + turns[nextTurnIndex]))
                {
                    x += turns[nextTurnIndex].x;
                    y += turns[nextTurnIndex].y;
                    currentTurnIndex += 1;
                }
                else
                {
                    x += turns[currentTurnIndex].x;
                    y += turns[currentTurnIndex].y;
                }
            }

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    previewTexture.SetPixel((int)(playerSpawnPosition.x + i), (int)(playerSpawnPosition.z + j), _playerSpawnPointColor);
                }
            }

            previewTexture.Apply();
        }
    }
}