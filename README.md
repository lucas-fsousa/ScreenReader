
# Screen Reader
The advanced screen reader allows the user to take a general screenshot of the main screen and locate small or large image snippets using AI for reading. Available for Linux and Windows.

#### Method structures and Parameters
- **ScreenSize** Stores the width and height of the screen.
- **PointIntoScreen** Stores X and Y coordinates of the screen.
- **BoxOfScreen** Represents the junction of the items "ScreenSize" and "PointIntoScreen"
  - **Filled** Returns the validation of the box to know if it is filled or not filled.
- **PixelColor** Represents the color of a screen pixel or an image.
- **imagePath** Represents the path to an image
- **confidence** How confident the script should be to signal that the image found is equal to the image provided in the path.

## Methods

### GetScreenSize
this method allows the user to identify the size of the current screen, that is, point X and point Y at their maximum, which we translate to W (Width) and H (Height) respectively.

**Example**:
```csharp
using PublicUtility.ScreenReader;
using PublicUtility.Nms.Structs;

// Simplified with "var"
var size1 = ScreenManager.GetScreenSize(); // returns a ScreenSize struct
Console.WriteLine(size1);

// Complete version
ScreenSize size2 = ScreenManager.GetScreenSize(); // returns a ScreenSize struct
Console.WriteLine(size2);
```

### PrintScreen
printscreen is a simple command capable of capturing the current screen in use. Its return is an Image of type "SixLabors.ImageSharp.Image" and can be converted to different types according to the user's preference.
**Example**
```csharp
using PublicUtility.ScreenReader;
using PublicUtility.Nms.Structs;
using SixLabors.ImageSharp;

// Example 1
var screenshotFull = ScreenManager.PrintScreen(); // get a printscreen in fullsize
screenshotFull.SaveAsPng(@"C:\Temp\fullScreenshot.png");

// Example 2
var initBox = new PointIntoScreen(X: 50, Y: 50); // x and y coordinates
var boxSize = new ScreenSize(Width: 200, Height: 200); // a box of 200 pixels X 200 pixels is formed
var box = new BoxOfScreen(boxSize, initBox);
var screenshotBox = ScreenManager.PrintScreen(box); // takes a screenshot starting from point X = 50 and point Y = 50 with a height of 200 pixels and a width of 200 pixels.
screenshotBox.SaveAsPng(@"C:\Temp\customScreenshot.png");

// Example 3 - Simplified
var screenshotBox2 = ScreenManager.PrintScreen(new(new(400, 400), new(100, 100))); // takes a screenshot starting from point X = 100 and point Y = 100 with a height of 400 pixels and a width of 400 pixels.
screenshotBox2.SaveAsPng(@"C:\Temp\customScreenshotSimplified.png");

```

### LocateOnScreen
this method is used to locate parts of images or colors on the user's main screen and has some ways to use it, see below.


It will search the screen until it finds a pixel that contains the desired color, if not, its return will be an empty coordinate point that indicates the non-location of the color on the screen

**Example for Windows only**
```csharp
using PublicUtility.ScreenReader;
using PublicUtility.Nms.Structs;

var color = new PixelColor(Alpha: 0, Red: 255, Green: 0, Blue: 0);
var loc = ScreenManager.LocateOnScreen(color);
Console.WriteLine(loc);

```

In this case, a search will be performed using an image fragment as a reference. If this image fragment used as a reference is on the screen, the function's return will be a Filled box that can be checked through the "Filled" read-only attribute that returns boolean for filled or not filled.

**Example**
```csharp
using PublicUtility.ScreenReader;

const string pathWindowsIco = @"C:\Temp\winIco.png";
var box = ScreenManager.LocateOnScreen(pathWindowsIco);
if(box.Filled)
  Console.WriteLine($"FILLED!");
else
  Console.WriteLine($"NOT FILLED!");
```

unlike LocateOnScreen, LocateAll returns all items located on the screen that have a shape within the standards and that are the same or similar to the image fragment provided in the search parameter.

**Example**
```csharp
using PublicUtility.ScreenReader;

const string pathWindowsIco = @"C:\Temp\winIco.png";
var listBox = ScreenManager.LocateAllOnScreen(pathWindowsIco);
foreach(var box in listBox)
  Console.WriteLine(box);

```

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License
[MIT](https://choosealicense.com/licenses/mit/)
