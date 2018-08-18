# House of Cards point cloud viewer

![](https://raw.githubusercontent.com/derouinw/house-of-cards/master/thom.PNG "Thom")

![](https://raw.githubusercontent.com/derouinw/house-of-cards/master/thominar.PNG "Thom in the Magic Leap")

Display of Radiohead's [House of Cards music video](https://www.youtube.com/watch?v=8nTFjVm9sTQ) point cloud data. Look at Thom Yorke's face in AR!

## Setup

### Downloading materials

`git clone git@github.com:derouinw/house-of-cards.git`

Open folder in Unity

> If you want to use it with the Magic Leap, make sure you follow the directions to [set it up](https://creator.magicleap.com/learn/guides/hardware-developer-quick-start)

Install the package [Point Cloud Free Viewer](https://assetstore.unity.com/packages/tools/utilities/point-cloud-free-viewer-19811)

Move the `PointCloudManager.cs` file from `Scripts` to `PointCloud/Scripts`, overwriting the one there.

> Feel free to use another solution for the point cloud, this is the one I got working the easiest. Note that it's a little old so you'll have to upgrade it to the newest Unity version, but it should still work

Download the [point cloud data](https://github.com/dataarts/radiohead)

> The two `HoC_AnimationData` files contain the point cloud data of Thom singing. The `HoC_DataApplications` file contains the city and culdesav point cloud data an audio sample, and more tools to view the data

### Converting data

The data is in a `.csv` format and needs to be converted to `.off` which is ready by the point cloud viewer. Fortunately, this just means adding a small amount of header information.

First, extract all `.csv` files to `Assets/Data` - the numbered files from `HoC_AnimationData` 1 and 2, and the city and culdesac files from `HoC_DataApplications/SceneViewer/Data`. Then, run the converter script in the directory to update all the files. This will create copies of all the files and may take a few minutes.

`python convert-csv-to-off.py`

Now, you should have numbered `.off` files from 1 to 2101, along with the city and culdesac.

### Setting up the scene

Add the `PointCloudManager` component to the `PointCloud` object

Set the `Manager` in the `GameController` to the object you just created

Set the `AudioClip` in the `Audio Source` to the song

> If you use the short clip included in the data, set the `GameController` `Initial Wait Time` to `0.0`. If you use your own copy of the original song, you'll need to fiddle with it a little. I clipped the song a little bit because the clip of Thom singing starts partway through - if you leave it at `14.7`, you'll want to clip the first 1:05 from the audio file. Otherwise, just adjust the start time.

### Generating the prefab objects from the point cloud data

The point cloud viewer will convert the data into quicker-loading prefab objects, but it will take some time the first time you run it. The data is saved into `Assets/Resources/PointCloudMeshes`.

Make sure `GameController Initial Load` is checked and click run. The audio should start playing and you'll see a bunch of loading bars running. It'll take a while to generate all the objects, there are 2101 frames.

> Be careful with stopping this process in the middle - if you try to run it again you may end up with error states from partially created objects. If this happens, delete any partially created objects and folders (or the entire folder) and start it again.

### Running on the Magic Leap One and other platforms

#### Magic Leap

The project is already set up with the magic leap project template, and you should follow the standard directions to point it at your sdk location and certificate. Then, it should run using the remote, or if you build and deploy it.

#### Steam VR

This project was originally built for Steam VR, and if you add the package it should also work. I haven't tested it with the bare setup, but you should just need to deactivate the main camera, and add a Steam VR camera.

## Potential future ideas

- Provide some sort of interactivity
- Make it easier to set up and use

## Thanks

Thank you to Radiohead for the song, and to Aaron Koblin and team for the original innovative music video. These days, LIDAR is pretty common technology but ten years ago it was very new!

Thank you to the entire Magic Leap team for building a very cool piece of technology that demonstrates that AR is clearly the future and is here to stay. Especially for creating an SDK that was so easy to work with (much easier than SteamVR).

Thank you also to Gerard Llorach for the point cloud viewer library and others who previously worked with the data.

## Contributing

Please feel free to submit pull requests to this repository. This was done in minimal free time on the side, so I know the code isn't perfect :)

If there are any issues with the setup instructions, please also let me know! I want to make sure this is as accessible to as many people as possible.

## License

This project under GNU GPLv3 (see LICENSE). Other projects referenced are under their own licenses.

By Bill Derouin ([@BillDerouin](https://twitter.com/BillDerouin)) and Greg Feingold ([@GregFeingold](https://twitter.com/GregFeingold)).
