using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[ExecuteAlways]
public class PaletteSwapper : MonoBehaviour
{
    [System.Serializable]
    public class ColorSwap
    {
        [Tooltip("The original color sampled from the sprite.")]
        public Color original = Color.white;

        [Tooltip("The color you want to replace it with.")]
        public Color replacement = Color.white;

        [Tooltip("Temporarily disable this swap without removing it.")]
        public bool enabled = true;
    }

    [Header("Shader Settings")]
    [Tooltip("How closely a pixel's color must match 'original' to be swapped (0 = exact match, increase for anti-aliased edges).")]
    [Range(0f, 0.15f)]
    public float threshold = 0.01f;

    [Header("Color Swaps")]
    public List<ColorSwap> swaps = new List<ColorSwap>();

    private SpriteRenderer _sr;

    private static readonly int SwapCountID = Shader.PropertyToID("_SwapCount");
    private static readonly int ThresholdID = Shader.PropertyToID("_Threshold");
    private static readonly int OriginalColorsID = Shader.PropertyToID("_OriginalColors");
    private static readonly int SwappedColorsID = Shader.PropertyToID("_SwappedColors");
    private const int MAX_SWAPS = 32;
    private readonly Vector4[] _originals    = new Vector4[MAX_SWAPS];
    private readonly Vector4[] _replacements = new Vector4[MAX_SWAPS];

    void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        Apply();
    }

    void OnValidate()
    {
        Apply();
    }

    public void Apply()
    {
        if (_sr == null) _sr = GetComponent<SpriteRenderer>();
        if (_sr == null) return;

        Material mat = Application.isPlaying ? _sr.material : _sr.sharedMaterial;

        if (mat == null)
        {
            Debug.LogWarning("[PaletteSwapper] No material assigned to SpriteRenderer.", this);
            return;
        }

        if (!mat.shader.name.Contains("PaletteSwap"))
        {
            Debug.LogWarning(
                $"[PaletteSwapper] Material shader is '{mat.shader.name}'. " +
                "Please assign a material that uses the Custom/PaletteSwap shader.", this);
            return;
        }

        int count = 0;
        foreach (var swap in swaps)
        {
            if (!swap.enabled) continue;
            if (count >= MAX_SWAPS) break;

            _originals[count]    = new Vector4(swap.original.r,    swap.original.g,    swap.original.b,    swap.original.a);
            _replacements[count] = new Vector4(swap.replacement.r, swap.replacement.g, swap.replacement.b, swap.replacement.a);
            count++;
        }

        mat.SetInt(SwapCountID, count);
        mat.SetFloat(ThresholdID, threshold);
        mat.SetVectorArray(OriginalColorsID, _originals);
        mat.SetVectorArray(SwappedColorsID,  _replacements);

        //Debug.Log($"[PaletteSwapper] Applied {count} active swap(s) to '{mat.name}'.");
    }

    public void AddSwap(Color original, Color replacement)
    {
        swaps.Add(new ColorSwap { original = original, replacement = replacement });
        Apply();
    }

    public void ClearSwaps()
    {
        swaps.Clear();
        Apply();
    }

    public List<Color> SampleSpriteColors(float alphaCutoff = 0.1f, int maxColors = 64)
    {
        var result = new List<Color>();
        if (_sr == null) _sr = GetComponent<SpriteRenderer>();
        if (_sr == null || _sr.sprite == null) return result;

        Sprite    sprite = _sr.sprite;
        Texture2D tex    = sprite.texture;

        Color[] pixels;
        try
        {
            Rect r = sprite.textureRect;
            pixels = tex.GetPixels((int)r.x, (int)r.y, (int)r.width, (int)r.height);
        }
        catch
        {
            Debug.LogWarning(
                $"[PaletteSwapper] '{tex.name}' is not Read/Write enabled. " +
                "Select the texture → Advanced → enable Read/Write → Apply.", this);
            return result;
        }

        var seen = new HashSet<Color32>();
        foreach (Color p in pixels)
        {
            if (p.a < alphaCutoff) continue;

            Color32 q = Quantize(p, 8);
            if (seen.Contains(q)) continue;

            seen.Add(q);
            result.Add(p);

            if (result.Count >= maxColors) break;
        }

        return result;
    }

    private static Color32 Quantize(Color c, int step)
    {
        byte Snap(float v) => (byte)(Mathf.RoundToInt(v * 255f / step) * step);
        return new Color32(Snap(c.r), Snap(c.g), Snap(c.b), 255);
    }
}