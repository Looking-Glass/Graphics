# Mask and detail maps

The High Definition Render Pipeline (HDRP) uses [channel-packed](Glossary.md#ChannelPacking) textures to store multiple Material maps in a single texture. Channel packing is efficient because it allows the renderer to sample up to four grayscale maps that use the same UV coordinates with a single texture fetch. HDRP uses two types of channel-packed textures: the [Mask Map](#MaskMap), and the [Detail Map](#DetailMap). They can use a different set of UV coordinates, or a different UV tiling value, giving you more creative freedom.

This document describes the format of the mask map and detail map so that you can author your own to use in HDRP.

To create a mask map:

1. Open image editing software that supports channel editing (such as Adobe Photoshop).
2. Import your grayscale textures onto separate layers.
3. Make only one of the layers visble.
4. Under **Adjustments**, select **Curves**.
5. Leaving only the color channel you want that texture to be in, set the outputs of the other channels to 0 and the inputs to 4. For information about which texture belongs in which channel, see [mask map](#MaskMap) and [detail map](#DetailMap).<br />![](Images/MaskMapAndDetailMap1.png)
6. Merge the curve with layer you've just worked on.
7. Repeat steps 3 to 6 for your other layers.
3. Export your image.
4. When you import the image into Unity, make sure that it uses linear color space and you set **Texture Type** to **Default**.

<a name="MaskMap"></a>

## Mask map

The mask map contains four grayscale textures, one in each color channel. The default expected value of each channel is 0.5. HDRP uses the mask map to store the metallic map, ambient occlusion map, detail mask, and smoothness map for the material. The mask map stores these textures in the following channels:

| **Color channel** | **Map**     |
| ----------------- | ----------- |
| **Red**           | Metallic    |
| **Green**         | Ambient Occlusion|
| **Blue**          | Detail mask |
| **Alpha**         | Smoothness  |

The following example image demonstrates the individual components of a full mask map.

![](Images/MaskMapAndDetailMap2.png)

<a name="DetailMap"></a>

## Detail map

The detail map enables you to overlay a second set of textures on top of the base surface information. Typically, the detail map scales several times across the object’s surface to add small details to a material. The detail map contains two grayscale textures and one two-component texture, which is the Material's detail normal map. When you import the detail map, disable the **sRGB** checkbox in the **Import Settings** window to make it work as expected.

| **Color channel** | **Map**            |
| ----------------- | ------------------ |
| **Red**           | Desaturated albedo |
| **Green**         | Normal Y           |
| **Blue**          | Smoothness         |
| **Alpha**         | Normal X           |

The following example image demonstrates the individual components of a full detail map.

![](Images/MaskMapAndDetailMap3.png)

### Desaturated albedo (red channel)

The red channel represents the albedo variation. It makes the underlying material's albedo gradually darken down to black when going from `0.5` to `0` or brighten up to white when going from `0.5` to `1`. A value of `0.5` is neutral, which means the detail map doesn't modify the albedo.

The image below shows the impact of the detail albedo on the final color. HDRP calculates color interpolation in sRGB space.

![](Images/DetailMap-red.png)

### Smoothness (blue channel)

The blue channel represents the smoothness variation and HDRP calculates it the same way as the albedo variation. The underlying material's smoothness gradually decreases if the detail smoothness is below `0.5` or increases if it's above `0.5`. A value of `0.5` is neutral, which means the detail map doesn't modify the smoothness.

The image below shows the impact of the detail smoothness on the final color.

![](Images/DetailMap-blue.png)

The following example shows the same gradient detail map as above, used by three Lit materials with different smoothness values.

![](Images/DetailMap-smoothness.png)
