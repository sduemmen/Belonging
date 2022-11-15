using System;
using Flags;
using UnityEngine;
using UnityEngine.UI;
using World;
using Random = System.Random;

namespace UI
{
    public class Minimap : MonoBehaviour
    {
        [SerializeField] private RawImage _minimap;
        [SerializeField] private Transform _player;
        [SerializeField] private Color _stoneColor = Color.gray;
        [SerializeField] private Color _treeColor = new Color(.23f, .45f, .28f, 1);
        [SerializeField] private Color _backgroundColor = new Color(.1f, .1f, .1f, .5f);
        [SerializeField] private Color _playerColor = new Color(.8f, .4f, .4f);
        private int _minimapWidth;
        private int _minimapHeight;

        private void Awake()
        {
            Rect rect = _minimap.rectTransform.rect;

            _minimapWidth = (int)rect.width / 2;
            _minimapHeight = (int)rect.height / 2;
            
            if (_minimap.texture == null)
            {
                Texture2D texture2D = new Texture2D(_minimapWidth, _minimapHeight) {
                    wrapMode = TextureWrapMode.Clamp,
                    filterMode = FilterMode.Point
                };

                _minimap.texture = texture2D;
            }
            
            InvokeRepeating(nameof(Draw), 0f, .5f);
        }

        private void Draw()
        {
            if (GameFlags.GAME_PAUSED) return;
            
            Texture2D _minimapTexture = (Texture2D)_minimap.texture;
            
            World.World world = World.World.Instance;
            Random random = new Random(world.seed);
            int xOffset = random.Next(-10000, 10000);
            int yOffset = random.Next(-10000, 10000);

            Vector3 playerPosition = _player.position;

            for (int x = 0; x < _minimapWidth; x++)
            {
                for (int y = 0; y < _minimapHeight; y++)
                {
                    float px = playerPosition.x + (x - _minimapWidth / 2);
                    float py = playerPosition.z + (y - _minimapHeight / 2);
                    float treeSample = WorldGenerator.SamplePerlin2d(px, py, xOffset, yOffset);
                    float stoneSample = WorldGenerator.SamplePerlin2d(px + 1000, py + 1000, xOffset, yOffset);
                    
                    if (treeSample < world.treeDensityThreshold && stoneSample < world.stoneDensityThreshold)
                    {
                        bool decider = Convert.ToBoolean(random.Next(0, 2));
                        Color color = decider ? _stoneColor : _treeColor;
                        _minimapTexture.SetPixel(x, y, color);
                        continue;
                    }

                    if (treeSample < world.treeDensityThreshold)
                    {
                        _minimapTexture.SetPixel(x, y, _treeColor);
                    }
                    else if (stoneSample < world.stoneDensityThreshold)
                    {
                        _minimapTexture.SetPixel(x, y, _stoneColor);
                    }
                    else
                    {
                        _minimapTexture.SetPixel(x, y, _backgroundColor);
                    }
                }
            }

            for (int x = -1; x < 1; x++)
            {
                for (int y = -1; y < 1; y++)
                {
                    _minimapTexture.SetPixel(x + _minimapWidth / 2, y + _minimapHeight/ 2, _playerColor);
                }
            }

            _minimapTexture.Apply();
        }
    }
}