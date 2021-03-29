//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Data;
//using System.Windows.Documents;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Navigation;
//using System.Windows.Shapes;

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32; //contains openfiledialog
using System.Collections.Generic; //contains list
using System.Drawing; //seems to require installing it
using System.IO; //memorystream
using System.Windows.Controls; //add textblock, label by code, image
using System.Diagnostics; //trace.writeline
//using System.Windows.Forms;
using System.Drawing.Imaging; //bitmapdata
using System.Runtime.InteropServices; //marshall
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using System.Windows.Interop;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;
using Image = System.Windows.Controls.Image;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using Point = System.Windows.Point;

// PROBLEM 1/2/3 = need to reopen picture that's in use <<IMPORTANT for users
// PROBLEM 4 = need to drag picture with precision = solved
// PROBLEM 5 = need to zoom picture at center of canvas (without mouseleftclick) <<IMPORTANT for users
// PROBLEM 6 - need to fix image relocation (and image resize)?
// PROBLEM 7: clear out image, rgbarrays
// PROBLEM 8: need to make this a class or nonglobal variables
// PROBLEM 9: optimize mouseleave                                               <<IMPORTANT for users
// PROBLEM 10: add brilliance filter to the image (to make edge detection generate more lines)


// IDEA = add undo-redo, original picture comparison button
// IDEA = add slider button to make adjusting intensity/darkness of edge detection more easier/visual
// IDEA = add picture rotation control
// IDEA = reset picture position/size when opening new picture/file
// IDEA = add AI photo-scaler



//renaming = rename file names in solution explorer, rename namespaces within cs files (must match with assembly name?), rename assembly info (found on properties), rename on file explorer, open up csproj (instead of solution)?

// CHECK OUT seemly-published app on C: \Users\patri\source\repos\ImageEditor_WPF\ImageEditor_WPF\bin\Debug\app.publish


//remove unnecessary codes, make a branch to show clean codes?

namespace _ImageEditor_WPF
{
    public partial class MainWindow : Window
    {

        // PROBLEM 8: need to make this a class or nonglobal variables
        WriteableBitmap currentimage2;
        int[] rgbArrayglobal;
        int[,] rgbArrayglobal_wh;
        int[,] rgbArrayglobal_wh_original;

        double liveheight;
        double livewidth;

        public MainWindow()
        {
            InitializeComponent();

            //-----------------------------
            //-----------------------------

            //shapes
            //System.Windows.Shapes.Rectangle shape1 = new System.Windows.Shapes.Rectangle();
            //shape1.Fill = System.Windows.Media.Brushes.Blue;
            //shape1.Width = 100;
            //shape1.Height = 100;
            //Canvas.SetTop(shape1, 20);
            //Canvas.SetLeft(shape1, 20);
            //shape1.MouseLeftButtonDown += Window_MouseLeftButtonDown; //uncomment this code if moving around rect, ellipse instead of image
            //Canvasx1.Children.Add(shape1);

            //-----------------------------
            //-----------------------------

            //file to open:
            //string filename = @"C:\Users\patri\OneDrive\Pictures\ahalle.png";
            string filename = @"C:\Users\patri\source\repos\PhotoStudyArtTool\PhotoStudyArtTool\images\baldur.jpg";
            //C: \Users\patri\source\repos\PhotoStudyArtTool\images_atroot
            //@"../../../images_atroot/baldur.jpg"


            //filename -> stream -> image -> writeablebitmap (confusing steps) - but it works
            Stream bmpStream = System.IO.File.Open(filename, System.IO.FileMode.Open);
            System.Drawing.Image image = System.Drawing.Image.FromStream(bmpStream);
            WriteableBitmap bmi2 = new WriteableBitmap(Bitmap2BitmapImage(new Bitmap(image))); //seems to preserve 

            //init global arrays
            rgbArrayglobal = new int[bmi2.PixelHeight * bmi2.PixelWidth];
            rgbArrayglobal_wh = new int[bmi2.PixelHeight, bmi2.PixelWidth];
            rgbArrayglobal_wh_original = new int[bmi2.PixelHeight, bmi2.PixelWidth];

            //info dimensions
            int width = bmi2.PixelWidth;
            int height = bmi2.PixelHeight;
            int stride = bmi2.BackBufferStride; //w * 4

            //fill in rgbArray4
            bmi2.CopyPixels(rgbArrayglobal, stride, 0);

            //make array[,] out of data from array[]
            int row_counter = 0;
            int col_counter = 0;
            for (int i = 0; i < width * height; i++)
            {
                rgbArrayglobal_wh[row_counter, col_counter] = rgbArrayglobal[i];
                rgbArrayglobal_wh_original[row_counter, col_counter] = rgbArrayglobal[i];
                col_counter++;

                // ex: row 0, col {ending number} to row 1, col 0
                if (col_counter > width - 1)
                {
                    col_counter = 0;
                    row_counter++;
                }
            }

            currentimage2 = bmi2;
            imgPhoto.Source = bmi2;

            tb1.Text = bmi2.PixelHeight.ToString() + " " + bmi2.PixelWidth.ToString();
            Trace.WriteLine(bmi2.Height.ToString() + " " + bmi2.Width.ToString());
            Trace.WriteLine(bmi2.PixelHeight.ToString() + " " + bmi2.PixelWidth.ToString() + " " + bmi2.BackBufferStride.ToString() + " " + bmi2.Format.BitsPerPixel.ToString());

            //set up liveheight/livewidth 
            Loaded += delegate
            {
                liveheight = imgPhoto.ActualHeight;
                livewidth = imgPhoto.ActualWidth;
            };


        }





        //FUNCTIONS 
        //FUNCTIONS
        //FUNCTIONS 
        //FUNCTIONS



        ////WORKS, not needed
        ////seems to work - Explore this! testing currently, v2
        ////try int[,] instead of int[] to solve the messy image result.
        ////not needed, replaced by writeablebitmap
        //private Bitmap InttoBitmap2(Bitmap bmp, int[,] rgbArray_wh)
        //{
        //    int w = bmp.Width;
        //    int h = bmp.Height;
        //    Bitmap bm = new Bitmap(w, h, PixelFormat.Format32bppArgb);

        //    Debug.WriteLine("inttobitmap2");
        //    for (int row = 0; row < h; row++)
        //    {
        //        for (int col = 0; col < w; col++)
        //        {
        //            System.Drawing.Color color = new System.Drawing.Color();
        //            color = System.Drawing.Color.FromArgb(rgbArray_wh[row, col]);


        //            if (row < 1 && col < 4)
        //                Debug.WriteLine(color.A.ToString() + " " + ((int)color.R).ToString() + " " + color.G.ToString() + " " + color.B.ToString());

        //            //way 1
        //            //bm.SetPixel(col, row, color);
        //            //way 2
        //            //bm.SetPixel(col, row, System.Drawing.Color.FromArgb(255, 0, 0, 120));
        //            //way 3
        //            //bm.SetPixel(col, row, System.Drawing.Color.FromArgb((int)color.A, (int)color.R, (int)color.G, (int)color.B));
        //            //way 4
        //            if ((int)color.R < 120 && (int)color.B < 120 && (int)color.G < 120)
        //            {
        //                bm.SetPixel(col, row, System.Drawing.Color.FromArgb((int)color.A, 0, 0, 0));
        //            }
        //            else
        //            {
        //                bm.SetPixel(col, row, System.Drawing.Color.FromArgb((int)color.A, (int)color.R, (int)color.G, (int)color.B));
        //            }
        //        }
        //    }
        //    return bm;
        //}

        //// WORKS, but kills Alpha, not needed
        //private Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        //{
        //    using (MemoryStream outStream = new MemoryStream())
        //    {
        //        BitmapEncoder enc = new BmpBitmapEncoder();
        //        enc.Frames.Add(BitmapFrame.Create(bitmapImage));
        //        enc.Save(outStream);
        //        Bitmap bitmap = new System.Drawing.Bitmap(outStream);

        //        return new Bitmap(bitmap);
        //    }
        //}



        // WORKS
        private BitmapImage Bitmap2BitmapImage(Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                return bitmapImage;
            }
        }


        //////Copy WriteableBitmap to ARGB byte array //idk
        //public static byte[] ToByteArray(this WriteableBitmap bmp)
        //{
        //    int[] p = bmp.Pixels;
        //    int len = p.Length * 4;
        //    byte[] result = new byte[len]; // ARGB
        //    Buffer.BlockCopy(p, 0, result, 0, len);
        //    return result;
        //}

        //////Copy ARGB byte array into WriteableBitmap
        //public static void FromByteArray(this WriteableBitmap bmp, byte[] buffer)
        //{
        //    Buffer.BlockCopy(buffer, 0, bmp.Pixels, 0, buffer.Length);
        //}







        //EVENT HANDLERS
        //EVENT HANDLERS
        //EVENT HANDLERS
        //EVENT HANDLERS

        // WORKING
        private void Click_loadapic(object sender, RoutedEventArgs e)
        {

            Microsoft.Win32.OpenFileDialog ab = new Microsoft.Win32.OpenFileDialog();
            ab.Title = "Pick an image";
            ab.Filter = "Image Files|*.jpg;*.bmp;*.png";
            if (ab.ShowDialog() == true)
            {
                currentimage2 = null;
                //atempt 1
                //atempt 1
                //currentimage = new BitmapImage(new Uri(ab.FileName));
                //imgPhoto.Source = currentimage; //commented out to try use writeablebitmap on imgPhoto.Source

                //preview
                //Debug.WriteLine(Path.GetFileNameWithoutExtension(ab.FileName) + Path.GetExtension(ab.FileName));

                ////Filename_Modified_To_Have_Png_Extension (fmthpe)
                //string fmthpe = (Path.GetFileNameWithoutExtension(ab.FileName).ToString() + ".png");



                //Bitmap bma = new Bitmap(ab.FileName);
                //Bitmap b = (Bitmap)Bitmap.FromStream(file.InputStream);

                //using (MemoryStream ms = new MemoryStream())
                //{
                //    b.Save(ms, ImageFormat.Png);

                //BitmapImage pbm = new BitmapImage(new Uri(ab.FileName));
                //atempt 1
                //atempt 1

                ////-----------------------------
                ////-----------------------------

                //atempt 2
                //atempt 2 
                ////works as making .jpg AND .png image opaque thru modifying alpha channel. Doesn't work on some JPG images however 
                //if (Path.GetExtension(ab.FileName) == ".jpg")
                //{
                //    Debug.WriteLine("it a jpg!!!!");
                //    //stream
                //}

                //else
                //{
                //    Debug.WriteLine("it non-jpg!!!!");
                //    WriteableBitmap bmi2 = new WriteableBitmap(new BitmapImage(new Uri(ab.FileName))); //seems to preserve tran
                //    currentimage2 = bmi2;
                //    imgPhoto.Source = bmi2;

                //}
                //atempt 2
                //atempt 2

                ////-----------------------------
                ////-----------------------------


                //attemp 3 how to turn image with any extension (bmp, jpg, png) into opaque image with modified alpha - solved
                //attemp 3


                //must produce image -> bitmap -> writeablebitmap lol
                Stream bmpStream = System.IO.File.Open(ab.FileName, System.IO.FileMode.Open); // PROBLEM 1... can't open image that's been used
                System.Drawing.Image image = System.Drawing.Image.FromStream(bmpStream);


                WriteableBitmap bmi2 = new WriteableBitmap(Bitmap2BitmapImage(new Bitmap(image))); //seems to preserve 


                //innit arrays
                rgbArrayglobal = new int[bmi2.PixelHeight * bmi2.PixelWidth];
                rgbArrayglobal_wh = new int[bmi2.PixelHeight, bmi2.PixelWidth];
                rgbArrayglobal_wh_original = new int[bmi2.PixelHeight, bmi2.PixelWidth];
                //info dimensions
                int width = bmi2.PixelWidth;
                int height = bmi2.PixelHeight;
                int stride = bmi2.BackBufferStride; //w * 4

                //fill in rgbArray4
                bmi2.CopyPixels(rgbArrayglobal, stride, 0);

                //make array[,] out of data from array[]
                int row_counter = 0;
                int col_counter = 0;
                for (int i = 0; i < width * height; i++)
                {
                    rgbArrayglobal_wh[row_counter, col_counter] = rgbArrayglobal[i];
                    rgbArrayglobal_wh_original[row_counter, col_counter] = rgbArrayglobal[i];
                    col_counter++;

                    // ex: row 0, col {ending number} to row 1, col 0
                    if (col_counter > width - 1)
                    {
                        col_counter = 0;
                        row_counter++;
                    }
                }

                //reset size and location
                imgPhoto.RenderTransform = new ScaleTransform(1, 1);
                Canvas.SetTop(imgPhoto, 0);
                Canvas.SetLeft(imgPhoto, 0);

                currentimage2 = bmi2;
                imgPhoto.Source = bmi2;


                tb1.Text = bmi2.PixelHeight.ToString() + " " + bmi2.PixelWidth.ToString() + " " + ab.FileName;
                Trace.WriteLine(bmi2.Height.ToString() + " " + bmi2.Width.ToString());
                Trace.WriteLine(bmi2.PixelHeight.ToString() + " " + bmi2.PixelWidth.ToString() + " " + bmi2.BackBufferStride.ToString() + " " + bmi2.Format.BitsPerPixel.ToString());

                //liveheight = currentimage2.PixelHeight * 1;
                //livewidth = currentimage2.PixelWidth * 1;
                //num1 = 1;

                //PROBLEM 2: how to clear out stream or opened file?
                //attemp 3
                //attemp 3
            }
        }


        //WIP, able to manipulate writeablebitmapimage, 
        // try rgbArray[,] in case u need it for edge detection algorithm
        // V5
        // should make it faster and more efficient (might need to centralize the rgb array, etc)
        private void Click_grayscale(object sender, RoutedEventArgs e)
        {

            ////attempt 12
            ////attempt 12 

            //info dimensions
            int width = currentimage2.PixelWidth;
            int height = currentimage2.PixelHeight;
            int stride = currentimage2.BackBufferStride; //w * 4

            ////check rgbArray4 = ok
            //Debug.WriteLine("rgbArrayglobal");
            //for (int i = 0; i < 4; i++)
            //    Debug.WriteLine(System.Drawing.Color.FromArgb(rgbArrayglobal[i]));
            //Debug.WriteLine("a " + System.Drawing.Color.FromArgb(rgbArrayglobal[0])); //row 0, col 0
            //Debug.WriteLine("a " + System.Drawing.Color.FromArgb(rgbArrayglobal[width - 1])); //row 0, col {end}
            //Debug.WriteLine("a " + System.Drawing.Color.FromArgb(rgbArrayglobal[width])); //row 1, col 0
            //Debug.WriteLine("a " + System.Drawing.Color.FromArgb(rgbArrayglobal[width*2 - 1])); //row 1, col {end}

            ////check rgbArray_getRGB4_wh = ok
            //Debug.WriteLine("rgbArrayglobal_wh");
            //for (int r = 0; r < 1; r++)
            //    for (int c = 0; c < 4; c++)
            //        Debug.WriteLine(System.Drawing.Color.FromArgb(rgbArrayglobal_wh[r,c]));
            //Debug.WriteLine("wh " + System.Drawing.Color.FromArgb(rgbArrayglobal_wh[0, 0])); //row 0, col 0
            //Debug.WriteLine("wh " + System.Drawing.Color.FromArgb(rgbArrayglobal_wh[0, width - 1]));  //row 0, col {end}
            //Debug.WriteLine("wh " + System.Drawing.Color.FromArgb(rgbArrayglobal_wh[1, 0])); //row 1, col 0
            //Debug.WriteLine("wh " + System.Drawing.Color.FromArgb(rgbArrayglobal_wh[1, width - 1]));  //row 1, col {end}

            //edit rgbArrayglobal
            for (int r = 0; r < height; r++)
            {
                for (int c = 0; c < width; c++)
                {
                    //get color info in integar form
                    int alpha = System.Drawing.Color.FromArgb(rgbArrayglobal_wh[r, c]).A;
                    int red = System.Drawing.Color.FromArgb(rgbArrayglobal_wh[r, c]).R;
                    int green = System.Drawing.Color.FromArgb(rgbArrayglobal_wh[r, c]).G;
                    int blue = System.Drawing.Color.FromArgb(rgbArrayglobal_wh[r, c]).B;

                    //adjustments

                    int bwcolor = (red + green + blue) / 3;
                    red = bwcolor;
                    green = bwcolor;
                    blue = bwcolor;

                    //keep ARGB within 0-255 range
                    if (alpha > 255) { alpha = 255; }
                    else if (alpha < 0) { alpha = 0; }
                    if (red > 255) { red = 255; }
                    else if (red < 0) { red = 0; }
                    if (green > 255) { green = 255; }
                    else if (green < 0) { green = 0; }
                    if (blue > 255) { blue = 255; }
                    else if (blue < 0) { blue = 0; }

                    //rgbArray4[i] = 259812; - not needed
                    // mylogic
                    // example: (255 << 24) A +    // A = 255
                    //          (100 << 16) R +    // R = 100
                    //          (200 << 16) R +    // G = 200
                    //          (255)              // B = 255 
                    rgbArrayglobal_wh[r, c] = (alpha << 24) +
                                   (red << 16) +
                                   (green << 8) +
                                   (blue);
                }

            }

            //make array[] from array[,]
            int counter = 0;
            for (int r = 0; r < height; r++)
            {
                for (int c = 0; c < width; c++)
                {
                    rgbArrayglobal[counter] = rgbArrayglobal_wh[r, c];
                    counter++;
                }
            }

            //apply edited rgbArray4 to wbitmap
            var rect1 = new Int32Rect(0, 0, width, height);
            currentimage2.WritePixels(rect1, rgbArrayglobal, stride, 0);

            //insert new wbitmap
            imgPhoto.Source = currentimage2;

            ////attempt 12
            ////attempt 12 
        }



        //v2
        private void Click_edgedetection(object sender, RoutedEventArgs e)
        {
            //info dimensions
            int height = currentimage2.PixelHeight;
            int width = currentimage2.PixelWidth;
            int stride = currentimage2.BackBufferStride;

            Debug.WriteLine("h:" + height + " w:" + width + " s:" + stride + " faka");
            //Debug.WriteLine("top left");
            //Debug.WriteLine(System.Drawing.Color.FromArgb(rgbArrayglobal[0]));
            //Debug.WriteLine(System.Drawing.Color.FromArgb(rgbArrayglobal_wh[0, 0]));
            //Debug.WriteLine("top right");
            //Debug.WriteLine(System.Drawing.Color.FromArgb(rgbArrayglobal[width-1]));
            //Debug.WriteLine(System.Drawing.Color.FromArgb(rgbArrayglobal_wh[0, width-1]));
            //Debug.WriteLine("bottom left");
            //Debug.WriteLine(System.Drawing.Color.FromArgb(rgbArrayglobal[width * height - (width)]));
            //Debug.WriteLine(System.Drawing.Color.FromArgb(rgbArrayglobal_wh[height-1, 0]));
            //Debug.WriteLine("bottom right");
            //Debug.WriteLine(System.Drawing.Color.FromArgb(rgbArrayglobal[width * height - 1]));
            //Debug.WriteLine(System.Drawing.Color.FromArgb(rgbArrayglobal_wh[height-1, width - 1]));


            int[,] array_gx = new int[3, 3];
            array_gx[0, 0] = -1; array_gx[0, 1] = 0; array_gx[0, 2] = 1;
            array_gx[1, 0] = -2; array_gx[1, 1] = 0; array_gx[1, 2] = 2;
            array_gx[2, 0] = -1; array_gx[2, 1] = 0; array_gx[2, 2] = 1;
            int[,] array_gy = new int[3, 3];
            array_gy[0, 0] = -1; array_gy[0, 1] = -2; array_gy[0, 2] = -1;
            array_gy[1, 0] = 0; array_gy[1, 1] = 0; array_gy[1, 2] = 0;
            array_gy[2, 0] = 1; array_gy[2, 1] = 2; array_gy[2, 2] = 1;

            //init
            int val_Alpha = 0;
            int val_Blue = 0;
            int val_Green = 0;
            int val_Red = 0;

            int Gx_sum_Blue = 0;
            int Gx_sum_Green = 0;
            int Gx_sum_Red = 0;

            int Gy_sum_Blue = 0;
            int Gy_sum_Green = 0;
            int Gy_sum_Red = 0;

            int Gxy_sum_final_Blue = 0;
            int Gxy_sum_final_Green = 0;
            int Gxy_sum_final_Red = 0;

            int logprint_sum1 = 0;

            //editing
            //editing
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Gx_sum_Blue = 0;
                    Gx_sum_Green = 0;
                    Gx_sum_Red = 0;

                    Gy_sum_Blue = 0;
                    Gy_sum_Green = 0;
                    Gy_sum_Red = 0;

                    Gxy_sum_final_Blue = 0;
                    Gxy_sum_final_Green = 0;
                    Gxy_sum_final_Red = 0;

                    //logprint_sum1++; //219600

                    // finding Gx and Gy - attempt 3 = good
                    // finding Gx and Gy - attempt 3
                    for (int r = 0; r < 3; r++)
                    {
                        for (int c = 0; c < 3; c++)
                        {
                            //logprint_sum1++; //219600 * 9 = 1976400 total (ahalle.png)
                            if (((i + (r - 1) >= 0) && (j + (c - 1) >= 0)) && ((i + (r - 1) < height) && (j + (c - 1) < width)))
                            {
                                int he = i + (r - 1);
                                int we = j + (c - 1);
                                val_Blue = System.Drawing.Color.FromArgb(rgbArrayglobal_wh_original[he, we]).B;
                                val_Green = System.Drawing.Color.FromArgb(rgbArrayglobal_wh_original[he, we]).G;
                                val_Red = System.Drawing.Color.FromArgb(rgbArrayglobal_wh_original[he, we]).R;
                                //logprint_sum1++; //1970710 (ahalle.png)
                            }

                            // if outside the border (row)
                            if ((i + (r - 1) < 0) || (i + (r - 1) > (height - 1)))
                            {
                                val_Blue = 0;
                                val_Green = 0;
                                val_Red = 0;
                                //logprint_sum1++; //2400
                            }

                            // if outside the border (column) 
                            // 400
                            else if ((j + (c - 1) < 0) || (j + (c - 1) > (width - 1)))
                            {
                                val_Blue = 0;
                                val_Green = 0;
                                val_Red = 0;
                                logprint_sum1++; //3290
                            }

                            Gx_sum_Blue += val_Blue * array_gx[r, c];
                            Gx_sum_Green += val_Green * array_gx[r, c];
                            Gx_sum_Red += val_Red * array_gx[r, c];

                            Gy_sum_Blue += val_Blue * array_gy[r, c];
                            Gy_sum_Green += val_Green * array_gy[r, c];
                            Gy_sum_Red += val_Red * array_gy[r, c];
                        }
                    }
                    // finding Gx and Gy - attempt 3
                    // finding Gx and Gy - attempt 3


                    Gxy_sum_final_Blue = (int)Math.Sqrt(((Math.Pow(Gx_sum_Blue, 2)) + (Math.Pow(Gy_sum_Blue, 2))));
                    Gxy_sum_final_Green = (int)(Math.Sqrt((Math.Pow(Gx_sum_Green, 2)) + (Math.Pow(Gy_sum_Green, 2))));
                    Gxy_sum_final_Red = (int)Math.Sqrt(((Math.Pow(Gx_sum_Red, 2)) + (Math.Pow(Gy_sum_Red, 2))));



                    int max = 255;
                    int min = 0;
                    if (Gxy_sum_final_Blue > max) { Gxy_sum_final_Blue = 255; }
                    if (Gxy_sum_final_Green > max) { Gxy_sum_final_Green = 255; }
                    if (Gxy_sum_final_Red > max) { Gxy_sum_final_Red = 255; }
                    if (Gxy_sum_final_Blue < min) { Gxy_sum_final_Blue = 0; }
                    if (Gxy_sum_final_Green < min) { Gxy_sum_final_Green = 0; }
                    if (Gxy_sum_final_Red < min) { Gxy_sum_final_Red = 0; }

                    //INVERT
                    Gxy_sum_final_Red = 255 - Gxy_sum_final_Red;
                    Gxy_sum_final_Green = 255 - Gxy_sum_final_Green;
                    Gxy_sum_final_Blue = 255 - Gxy_sum_final_Blue;
                    Gxy_sum_final_Red = (Gxy_sum_final_Red + Gxy_sum_final_Green + Gxy_sum_final_Blue) / 3; //
                    Gxy_sum_final_Green = (Gxy_sum_final_Red + Gxy_sum_final_Green + Gxy_sum_final_Blue) / 3;//
                    Gxy_sum_final_Blue = (Gxy_sum_final_Red + Gxy_sum_final_Green + Gxy_sum_final_Blue) / 3;//


                    val_Alpha = System.Drawing.Color.FromArgb(rgbArrayglobal_wh_original[i, j]).A;


                    int bw_cutoff = 200; //should try make a slider ouit of this //IDEA1
                    //make alpha=0 background
                    if (Gxy_sum_final_Blue >= bw_cutoff && Gxy_sum_final_Blue >= bw_cutoff && Gxy_sum_final_Blue >= bw_cutoff)
                    {
                        val_Alpha = 0;

                        //OR
                        //Gxy_sum_final_Blue = 235;
                        //Gxy_sum_final_Green = 235;
                        //Gxy_sum_final_Red = 235;

                    }
                    else
                    {
                        //Gxy_sum_final_Blue = (int)(Gxy_sum_final_Blue * 0.4); //should make sliders out of these //IDEA2 ex: 100% = 1, 200% = 2
                        //Gxy_sum_final_Green = (int)(Gxy_sum_final_Green * 0.4);
                        //Gxy_sum_final_Red = (int)(Gxy_sum_final_Red * 0.4);
                    }


                    //make remaining non-alpha pixel blacker
                    int max2 = 120;
                    if (Gxy_sum_final_Blue > max2) { Gxy_sum_final_Blue = (int)(Gxy_sum_final_Blue * 0.4); }
                    if (Gxy_sum_final_Green > max2) { Gxy_sum_final_Green = (int)(Gxy_sum_final_Green * 0.4); }
                    if (Gxy_sum_final_Red > max2) { Gxy_sum_final_Red = (int)(Gxy_sum_final_Red * 0.4); }

                    //BW
                    int bwcolor = (Gxy_sum_final_Blue + Gxy_sum_final_Green + Gxy_sum_final_Red) / 3;
                    Gxy_sum_final_Blue = bwcolor;
                    Gxy_sum_final_Green = bwcolor;
                    Gxy_sum_final_Red = bwcolor;


                    rgbArrayglobal_wh[i, j] = (val_Alpha << 24) +
                                (Gxy_sum_final_Red << 16) +
                                (Gxy_sum_final_Green << 8) +
                                (Gxy_sum_final_Blue);
                }
            }
            //editing
            //editing


            ////corner check - preview
            ////corner check
            //// A R G B
            //int color1 = (255 << 24) + (255 << 16) + (100 << 8) + (100); //red
            //int color2 = (255 << 24) + (100 << 16) + (255 << 8) + (100); //green
            //int color3 = (255 << 24) + (100 << 16) + (100 << 8) + (255); //blue
            //int color4 = (255 << 24) + (255 << 16) + (150 << 8) + (150); //pink
            //Debug.WriteLine("h:" + height + " w:" + width + " s:" + stride + " faka");

            //Debug.WriteLine("top left");
            //rgbArrayglobal_wh[0, 0] = color1;
            //Debug.WriteLine(System.Drawing.Color.FromArgb(rgbArrayglobal[0]));
            //Debug.WriteLine(System.Drawing.Color.FromArgb(rgbArrayglobal_wh[0, 0]));

            //Debug.WriteLine("top right");
            //rgbArrayglobal_wh[0, width - 1] = color2;
            //Debug.WriteLine(System.Drawing.Color.FromArgb(rgbArrayglobal[width - 1]));
            //Debug.WriteLine(System.Drawing.Color.FromArgb(rgbArrayglobal_wh[0, width - 1]));

            //Debug.WriteLine("bottom left");
            //rgbArrayglobal_wh[height - 1, 0] = color3;
            //Debug.WriteLine(System.Drawing.Color.FromArgb(rgbArrayglobal[width * height - (width)]));
            //Debug.WriteLine(System.Drawing.Color.FromArgb(rgbArrayglobal_wh[height - 1, 0]));

            //Debug.WriteLine("bottom right");
            //rgbArrayglobal_wh[height - 1, width - 1] = color4;
            //Debug.WriteLine(System.Drawing.Color.FromArgb(rgbArrayglobal[width * height - 1]));
            //Debug.WriteLine(System.Drawing.Color.FromArgb(rgbArrayglobal_wh[height - 1, width - 1]));
            ////corner check
            ////corner check



            //make array[] from array[,]
            int counter = 0;
            for (int r = 0; r < height; r++)
            {
                for (int c = 0; c < width; c++)
                {
                    rgbArrayglobal[counter] = rgbArrayglobal_wh[r, c];
                    counter++;
                }
            }

            Debug.WriteLine("check rgbArrayglobal");
            for (int i = 0; i < 4; i++)
                Debug.WriteLine(System.Drawing.Color.FromArgb(rgbArrayglobal[i]));
            Debug.WriteLine("check rgbArrayglobal_wh");
            for (int i = 0; i < 1; i++)
                for (int j = 0; j < 4; j++)
                    Debug.WriteLine(System.Drawing.Color.FromArgb(rgbArrayglobal_wh[i, j]));
            Debug.WriteLine("check rgbArrayglobal_wh_original");
            for (int i = 0; i < 1; i++)
                for (int j = 0; j < 4; j++)
                    Debug.WriteLine(System.Drawing.Color.FromArgb(rgbArrayglobal_wh_original[i, j]));


            //apply edited rgbArray4 to wbitmap
            var rect1 = new Int32Rect(0, 0, width, height);
            currentimage2.WritePixels(rect1, rgbArrayglobal, stride, 0);


            //update image source
            imgPhoto.Source = currentimage2;

            //update original way1
            for (int r = 0; r < height; r++)
                for (int c = 0; c < width; c++)
                    rgbArrayglobal_wh_original[r, c] = rgbArrayglobal_wh[r, c];
            ////update original way2 - bad way
            //rgbArrayglobal_wh_original = rgbArrayglobal_wh;

            Debug.Write(logprint_sum1 + "count");
        }









        /// //////////////////
        /// //////////////////



        private void printlog(System.Windows.Input.MouseEventArgs e)
        {
            //compute precise position x,y for  settop, setleft
            Debug.WriteLine("");
            Debug.WriteLine("canvas (0,0): 0 (y) | 0 (x) = fixed"); // good //for centered zoom in
            Debug.WriteLine("canvas (mouse x,y): " + (e.GetPosition(Canvasx1).Y).ToString() + "(y) | " + (e.GetPosition(Canvasx1).X).ToString() + "(x) = adj");
            Debug.WriteLine("Canvas ActualH/W: " + (Canvasx1.ActualHeight).ToString() + "(y) | " + (Canvasx1.ActualWidth).ToString() + "(x) = fixed");

            double bmptop = e.GetPosition(Canvasx1).Y - liveheight / 2;
            double bmpleft = e.GetPosition(Canvasx1).X - livewidth / 2;
            Debug.WriteLine("BMP (top/left x,y): " + (bmptop).ToString() + "(y) | " + (bmpleft).ToString() + "(x) = adj");
            Debug.WriteLine("BMP PixelH/W: " + (currentimage2.PixelHeight).ToString() + "(y) | " + (currentimage2.PixelWidth).ToString() + "(x) = fixed");
            Debug.WriteLine("BMP liveH/W: " + (liveheight).ToString() + "(y) | " + (livewidth).ToString() + "(x) = adj");

            //image seems to have dimension of canvas
            Debug.WriteLine("imgPhoto (top/left x,y): " + (Canvas.GetTop(imgPhoto)).ToString() + "(y) | " + (Canvas.GetLeft(imgPhoto)).ToString() + "(x) = (x,y) on canvas"); //for centered zoom in
            Debug.WriteLine("imgPhotoH/W: " + (imgPhoto.Height).ToString() + "(y) | " + (imgPhoto.Width).ToString() + "(x) = fixed, matches canvas ActualH/W");
            Debug.WriteLine("imgPhoto ActualH/W: " + (imgPhoto.ActualHeight).ToString() + "(y) | " + (imgPhoto.ActualWidth).ToString() + "(x) = fixed, scaled-to-fit-canvas from bmp pixelH/W");

            Debug.WriteLine("num1: " + num1);
            Debug.WriteLine("Adj: " + adjy + "(y) | " + adjx + "(x)");
            Debug.WriteLine("Dist: " + disty + "(y) | " + distx + "(x)");
            Debug.WriteLine("Origin: " + originy + "(y) | " + originx + "(x)");

            //for centered zoom in
            Debug.WriteLine("imgPhoto (top/left x,y): " + (Canvas.GetTop(imgPhoto)).ToString() + "(y) | " + (Canvas.GetLeft(imgPhoto)).ToString() + "(x) = (x,y) on canvas"); //for centered zoom in
            disty2_original = 0 - Canvas.GetTop(imgPhoto);
            distx2_original = 0 - Canvas.GetLeft(imgPhoto);
            Debug.WriteLine("Dist2: " + disty2_original + "(y) | " + distx2_original + "(x)");
            disty2_new = 0 - Canvas.GetTop(imgPhoto) * num1;
            distx2_new = 0 - Canvas.GetLeft(imgPhoto) * num1;
            Debug.WriteLine("Dist2: " + disty2_new + "(y) | " + distx2_new + "(x)");

            //imgphoto_centralpty = Canvas.GetTop(imgPhoto) + (imgPhoto.ActualHeight / 2) + adjy;
            //imgphoto_centralptx = Canvas.GetLeft(imgPhoto) + (imgPhoto.ActualWidth / 2) + adjx;
            //Debug.WriteLine("imgphoto_centralpt: " + imgphoto_centralpty + "(y) | " + imgphoto_centralptx + "(x)");
            ////for centered zoom in

            //imgphoto_focalpty = imgphoto_centralpty + disty2_new;
            //imgphoto_focalptx = imgphoto_centralptx + distx2_new;
            //Debug.WriteLine("imgphoto_focalpt: " + imgphoto_focalpty + "(y) | " + imgphoto_focalptx + "(x)");

            //imgphoto_centralpty = imgPhoto.ActualHeight / 2;
            //imgphoto_centralptx = imgPhoto.ActualWidth / 2;
            //Debug.WriteLine("imgphoto_centralpt: " + imgphoto_centralpty + "(y) | " + imgphoto_centralptx + "(x)");
            ////for centered zoom in

            //imgphoto_focalpty = imgphoto_centralpty + disty2_new;
            //imgphoto_focalptx = imgphoto_centralptx + distx2_new ;
            //Debug.WriteLine("imgphoto_focalpt: " + imgphoto_focalpty + "(y) | " + imgphoto_focalptx + "(x)");
            ////for centered zoom in

        }



        // WORKING
        double num1 = 1;
        private void imgPhoto_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            //rotate a photo WAY 2
            if (num1 >= 0.5)
            {
                if (e.Delta > 0)
                {
                    num1 *= 1.25;
                    imgPhoto.RenderTransform = new ScaleTransform(num1, num1);

                }

                else if (e.Delta < 0)
                {

                    num1 /= 1.25;
                    imgPhoto.RenderTransform = new ScaleTransform(num1, num1);
                }
            }
            else
            {
                num1 = 0.50;
            }


            liveheight = imgPhoto.ActualHeight * num1;
            livewidth = imgPhoto.ActualWidth * num1;

        }






        bool down = false;
        //or
        //UIElement dragObject = null;
        //ImageSource dragObject = null;
        //Point offset;
        double disty;
        double distx;
        double adjy;
        double adjx;
        double originy;
        double originx;


        double disty2_original;
        double distx2_original;
        double disty2_new;
        double distx2_new;
        double imgphoto_centralpty;
        double imgphoto_centralptx;
        double imgphoto_focalpty;
        double imgphoto_focalptx;

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            down = true;
            // disty/x = distance between imgPhoto (0,0) and imgPhoto (mouse x,y)
            disty = e.GetPosition(imgPhoto).Y;
            distx = e.GetPosition(imgPhoto).X;
            // adjy/x = distance between imgPhoto.ActualHeight/Width and imgPhoto.Height/Width
            adjy = (imgPhoto.Height - imgPhoto.ActualHeight) / 2;
            adjx = (imgPhoto.Width - imgPhoto.ActualWidth) / 2;
            // for RenderTransformOrigin, to center transform at the current mouse (x,y) of canvas/windows
            originy = disty / imgPhoto.ActualHeight;
            originx = distx / imgPhoto.ActualWidth;



            // moved to mousemove
            //imgPhoto.RenderTransformOrigin = new Point(originx, originy); 

            //or - not needed
            //this.dragObject = sender as UIElement;
            //this.offset = e.GetPosition(Canvasx1);
            //this.offset.Y -= Canvas.GetTop(this.dragObject);
            //this.offset.X -= Canvas.GetLeft(this.dragObject);
            //this.Canvasx1.CaptureMouse();
            ////or - not needed
            //dragObject = sender as UIElement;
            //offset = e.GetPosition(Canvasx1);
            //offset.Y -= Canvas.GetTop(this.dragObject);
            //offset.X -= Canvas.GetLeft(this.dragObject);
            //Canvasx1.CaptureMouse();
            //or - not needed
            //dragObject = sender as ImageSource;
            //offset = e.GetPosition(Canvasx1);
            //offset.Y -= Canvas.GetTop(dragObject);
            //offset.X -= Canvas.GetLeft(dragObject);
            //Canvasx1.CaptureMouse();
        }

        private void Window_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            down = false;
            //disty = imgphoto_focalpty;
            //distx = imgphoto_focalptx;
            //originy = disty / imgPhoto.ActualHeight;
            //originx = distx / imgPhoto.ActualWidth;


            // or - not needed
            //this.dragObject = null;
            //this.Canvasx1.ReleaseMouseCapture();
            //// or - not needed
            //dragObject = null;
            //this.Canvasx1.ReleaseMouseCapture();


        }


        // PROBLEM 9: optimize mouseleave, may not need it
        private void Window_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            down = false;

            //or - not needed
            //this.dragObject = null;
            //this.Canvasx1.ReleaseMouseCapture();
            ////or - not needed
            //dragObject = null;
            //Canvasx1.ReleaseMouseCapture();
        }



        private void imgPhoto_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (down == true)
            {
                printlog(e);

                //SETTOP, SETLEFT
                //Canvas.SetTop(imgPhoto, e.GetPosition(Canvasx1).Y - imgPhoto.Height / 2);
                //Canvas.SetLeft(imgPhoto, e.GetPosition(Canvasx1).X - imgPhoto.Width / 2);
                Canvas.SetTop(imgPhoto, e.GetPosition(Canvasx1).Y - (adjy + disty));
                Canvas.SetLeft(imgPhoto, e.GetPosition(Canvasx1).X - (adjx + distx));
                imgPhoto.RenderTransformOrigin = new Point(originx, originy);
                //OR
                //TranslateTransform transform = new TranslateTransform();
                //transform.Y = e.GetPosition(Canvasx1).Y - (adjy + disty);
                //transform.X = e.GetPosition(Canvasx1).X - (adjx + distx);
                //imgPhoto.RenderTransform = transform;

            }

            //// or - not needed
            //if (this.dragObject == null)
            //    return;
            //var position = e.GetPosition(sender as IInputElement);
            //Canvas.SetTop(this.dragObject, position.Y - this.offset.Y);
            //Canvas.SetLeft(this.dragObject, position.X - this.offset.X);
            //// or - not needed
            //if (dragObject == null)
            //    return;
            //var position = e.GetPosition(sender as IInputElement);
            //Canvas.SetTop(dragObject, position.Y - offset.Y);
            //Canvas.SetLeft(dragObject, position.X - offset.X);
        }




        //PROBLEM 6 - need to fix image relocation (and image resize)?
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //way 2
            //adjust size
            double heightratio = imgPhoto.ActualHeight / Canvasx1.ActualHeight;
            double widthratio = imgPhoto.ActualWidth / Canvasx1.ActualWidth;

            imgPhoto.Height = imgPhoto.ActualHeight / heightratio;
            imgPhoto.Width = imgPhoto.ActualWidth / widthratio;


            //adjust location
            //Canvas.SetTop(imgPhoto, Canvas.GetTop(imgPhoto) * heightratio);
            //Canvas.SetLeft(imgPhoto, Canvas.GetLeft(imgPhoto) * widthratio);
        }




        //save
        //save
        //WORKING
        //need to save into custom location or open up dialog - solved
        //need to save as, save button 
        void CreateThumbnail(string filename, BitmapSource image5)
        {
            if (filename != string.Empty)
            {

                //way1 /
                //using (FileStream stream5 = new FileStream(filename, FileMode.OpenOrCreate))
                //{
                //    PngBitmapEncoder encoder5 = new PngBitmapEncoder();
                //    encoder5.Frames.Add(BitmapFrame.Create(image5));
                //    encoder5.Save(stream5);
                //}

                //way2 - PROBLEM3= need to overcome repetitive opening same file???????
                try
                {
                    FileStream stream5 = new FileStream(filename, FileMode.OpenOrCreate);
                    PngBitmapEncoder encoder5 = new PngBitmapEncoder();
                    encoder5.Frames.Add(BitmapFrame.Create(image5));
                    encoder5.Save(stream5);
                    stream5.Close(); //not needed 
                }
                catch
                {
                    //reinitalize picture? //just reuse pic?
                }
            }
        }
        private void Click_save(object sender, RoutedEventArgs e)
        {
            ////way 1 = works (good for save button, should overwrite stored, opened file name) 
            //CreateThumbnail(@"C:\Users\patri\Downloads\banana_edited.png", currentimage2);
            ////now try to open dialog

            //way 2 = works (good for save as button)
            Microsoft.Win32.SaveFileDialog ab = new Microsoft.Win32.SaveFileDialog();
            ab.Title = "save a pic";
            ab.Filter = "Image Files|*.png;*.jpg;*.bmp;"; // nneed to support auto/manual extension naming????
            if (ab.ShowDialog() == true)
                CreateThumbnail(ab.FileName, currentimage2);
        }
        //save
        //save


        // FEATURE TO DOS:
        // FEATURE TO DOS:
        // FEATURE TO DOS:

        private void Click_reset(object sender, RoutedEventArgs e)
        {
        }

        private void Click_preview(object sender, RoutedEventArgs e)
        {
        }

        // PROBLEM 7: clear out image, rgbarrays
        private void Click_remove(object sender, RoutedEventArgs e)
        {
            imgPhoto.Source = new BitmapImage();
            currentimage2 = null;
        }

        private void Click_redo(object sender, RoutedEventArgs e)
        {
        }

        private void Click_undo(object sender, RoutedEventArgs e)
        {
        }

        private void Click_2dsphereadd(object sender, RoutedEventArgs e)
        {
        }

    }



}











//need to delete ones with local rgb Array
//----------------------------------------------------------------------

//v1
//private void Click_edgedetection(object sender, RoutedEventArgs e)
//{
//    WriteableBitmap wbitmap = new WriteableBitmap(currentimage2);


//    tb1.Text = wbitmap.PixelHeight.ToString() + " " + wbitmap.PixelWidth.ToString();
//    Trace.WriteLine(wbitmap.Height.ToString() + " " + wbitmap.Width.ToString());
//    Trace.WriteLine(wbitmap.PixelHeight.ToString() + " " + wbitmap.PixelWidth.ToString() + " " + wbitmap.BackBufferStride.ToString() + " " + wbitmap.Format.BitsPerPixel.ToString());

//    //initialize array
//    int[] rgbArray4 = new int[wbitmap.PixelHeight * wbitmap.PixelWidth]; //have to use pixelwidth when using writeablebitmap, idk why
//    int[,] rgbArray4_wh = new int[wbitmap.PixelHeight, wbitmap.PixelWidth]; //have to use pixelwidth when using writeablebitmap, idk why
//    int[,] rgbArray4_wh_original = new int[wbitmap.PixelHeight, wbitmap.PixelWidth];




//    //info dimensions
//    int width = wbitmap.PixelWidth;
//    int height = wbitmap.PixelHeight;
//    int stride = wbitmap.BackBufferStride; //w * 4

//    //fill in rgbArray4
//    wbitmap.CopyPixels(rgbArray4, stride, 0);

//    //make array[,] out of data from array[]
//    int row_counter = 0;
//    int col_counter = 0;
//    for (int i = 0; i < width * height; i++)
//    {
//        rgbArray4_wh[row_counter, col_counter] = rgbArray4[i];
//        rgbArray4_wh_original[row_counter, col_counter] = rgbArray4[i];
//        col_counter++;

//        // ex: row 0, col {ending number} to row 1, col 0
//        if (col_counter > width - 1)
//        {
//            col_counter = 0;
//            row_counter++;
//        }
//    }


//    //copy rgbArray4_wh
//    //rgbArray4_wh_original = rgbArray4_wh; // seems to ok
//    Debug.WriteLine("check rgbArray4");
//    for (int i = 0; i < 4; i++)
//        Debug.WriteLine(System.Drawing.Color.FromArgb(rgbArray4[i]));
//    Debug.WriteLine("check rgbArray4_wh");
//    for (int i = 0; i < 1; i++)
//        for (int j = 0; j < 4; j++)
//            Debug.WriteLine(System.Drawing.Color.FromArgb(rgbArray4_wh[i, j]));
//    Debug.WriteLine("check rgbArray4_wh_original");
//    for (int i = 0; i < 1; i++)
//        for (int j = 0; j < 4; j++)
//            Debug.WriteLine(System.Drawing.Color.FromArgb(rgbArray4_wh_original[i, j]));



//    //edit - apply ED
//    //edit - apply ED
//    int[,] array_gx = new int[3, 3];
//    int[,] array_gy = new int[3, 3];
//    array_gx[0, 0] = -1; array_gx[0, 1] = 0; array_gx[0, 2] = 1;
//    array_gx[1, 0] = -2; array_gx[1, 1] = 0; array_gx[1, 2] = 2;
//    array_gx[2, 0] = -1; array_gx[2, 1] = 0; array_gx[2, 2] = 1;


//    array_gy[0, 0] = -1; array_gy[0, 1] = -2; array_gy[0, 2] = -1;
//    array_gy[1, 0] = 0; array_gy[1, 1] = 0; array_gy[1, 2] = 0;
//    array_gy[2, 0] = 1; array_gy[2, 1] = 2; array_gy[2, 2] = 1;


//    //array_gx[0, 0] = -1; array_gx[0, 1] = -2; array_gx[0, 2] = -1;
//    //array_gx[1, 0] = 0; array_gx[1, 1] = 0; array_gx[1, 2] = 0;
//    //array_gx[2, 0] = 1; array_gx[2, 1] = 2; array_gx[2, 2] = 1;

//    //array_gy[0, 0] = 1; array_gy[0, 1] = 0; array_gy[0, 2] = -1;
//    //array_gy[1, 0] = 2; array_gy[1, 1] = 0; array_gy[1, 2] = -2;
//    //array_gy[2, 0] = 1; array_gy[2, 1] =0; array_gy[2, 2] = -1;



//    //init
//    int val_Alpha = 0;
//    int val_Blue = 0;
//    int val_Green = 0;
//    int val_Red = 0;

//    int Gx_sum_Blue = 0;
//    int Gx_sum_Green = 0;
//    int Gx_sum_Red = 0;

//    int Gy_sum_Blue = 0;
//    int Gy_sum_Green = 0;
//    int Gy_sum_Red = 0;

//    int Gxy_sum_final_Blue = 0;
//    int Gxy_sum_final_Green = 0;
//    int Gxy_sum_final_Red = 0;

//    int logprint_sum1 = 0;

//    //rgbArray4_wh_original = rgbArray4_wh; // seems to ok
//    Debug.Write("h:" + height + " w:" + width);


//    //editing
//    //editing
//    for (int H = 0; H < height; H++)
//    {
//        for (int W = 0; W < width; W++)
//        {
//            Gx_sum_Blue = 0;
//            Gx_sum_Green = 0;
//            Gx_sum_Red = 0;

//            Gy_sum_Blue = 0;
//            Gy_sum_Green = 0;
//            Gy_sum_Red = 0;

//            Gxy_sum_final_Blue = 0;
//            Gxy_sum_final_Green = 0;
//            Gxy_sum_final_Red = 0;


//            //logprint_sum1++; //219600


//            //// finding Gx and Gy - attempt 1
//            //// finding Gx and Gy - attempt 1
//            //for (int r = 0; r < 3; r++)
//            //{
//            //    for (int c = 0; c < 3; c++)
//            //    {
//            //        //logprint_sum1++; //219600 * 9 = 1976400 total (ahalle.png)

//            //        // if outside the border (row)
//            //        if ((H + (r - 1) < 0) || (H + (r - 1) > (height - 1)))
//            //        {
//            //            val_Blue = 0;
//            //            val_Green = 0;
//            //            val_Red = 0;
//            //            //logprint_sum1++; //2400
//            //        }

//            //        // if outside the border (column) 
//            //        // 400
//            //        else if ((W + (c - 1) < 0) || (W + (c - 1) > (width - 1)))
//            //        {
//            //            val_Blue = 0;
//            //            val_Green = 0;
//            //            val_Red = 0;
//            //            //logprint_sum1++; //3290
//            //        }
//            //        else
//            //        {
//            //            //// NOT PERFECT - NEED FIX!
//            //            if (((H + (r - 1) > 0) && (W + (c - 1) > 0)) || ((H + (r - 1) < height) && (W + (c - 1) < width)))
//            //            {
//            //                int he = H + (r - 1);
//            //                int we = W + (c - 1);
//            //                //if (he > height - 1) { he = height - 1; }
//            //                //if (we > width - 1) { we = width - 1; }
//            //                //if (he < 0) { he = 0; }
//            //                //if (we < 0) { we = 0; }
//            //                val_Blue = System.Drawing.Color.FromArgb(rgbArray4_wh_original[he, we]).B;
//            //                val_Green = System.Drawing.Color.FromArgb(rgbArray4_wh_original[he, we]).G;
//            //                val_Red = System.Drawing.Color.FromArgb(rgbArray4_wh_original[he, we]).R;
//            //                //logprint_sum1++; 1970710 (ahalle.png)
//            //            }
//            //        }

//            //        Gx_sum_Blue += val_Blue * array_gx[r, c];
//            //        Gx_sum_Green += val_Green * array_gx[r, c];
//            //        Gx_sum_Red += val_Red * array_gx[r, c];

//            //        Gy_sum_Blue += val_Blue * array_gy[r, c];
//            //        Gy_sum_Green += val_Green * array_gy[r, c];
//            //        Gy_sum_Red += val_Red * array_gy[r, c];
//            //    }
//            //}
//            //// finding Gx and Gy - attfinding Gx and Gy - attempt 1empt 1
//            //// 




//            // finding Gx and Gy - attempt 2
//            // finding Gx and Gy - attempt 2
//            for (int r = 0; r < 3; r++)
//            {
//                for (int c = 0; c < 3; c++)
//                {
//                    //logprint_sum1++; //219600 * 9 = 1976400 total (ahalle.png)



//                    // if outside the border (row)
//                    if ((H + (r - 1) < 0) || (H + (r - 1) > (height - 1)))
//                    {
//                        val_Blue = 0;
//                        val_Green = 0;
//                        val_Red = 0;
//                        //logprint_sum1++; //2400
//                    }

//                    // if outside the border (column) 
//                    // 400
//                    else if ((W + (c - 1) < 0) || (W + (c - 1) > (width - 1)))
//                    {
//                        val_Blue = 0;
//                        val_Green = 0;
//                        val_Red = 0;
//                        //logprint_sum1++; //3290
//                    }
//                    else
//                    {
//                        if (((H + (r - 1) >= 0) && (W + (c - 1) >= 0)) || ((H + (r - 1) < height) && (W + (c - 1) < width)))
//                        {
//                            int he = H + (r - 1);
//                            int we = W + (c - 1);
//                            val_Blue = System.Drawing.Color.FromArgb(rgbArray4_wh_original[he, we]).B;
//                            val_Green = System.Drawing.Color.FromArgb(rgbArray4_wh_original[he, we]).G;
//                            val_Red = System.Drawing.Color.FromArgb(rgbArray4_wh_original[he, we]).R;
//                            //logprint_sum1++; 1970710 (ahalle.png)
//                        }
//                    }

//                    Gx_sum_Blue += val_Blue * array_gx[r, c];
//                    Gx_sum_Green += val_Green * array_gx[r, c];
//                    Gx_sum_Red += val_Red * array_gx[r, c];

//                    Gy_sum_Blue += val_Blue * array_gy[r, c];
//                    Gy_sum_Green += val_Green * array_gy[r, c];
//                    Gy_sum_Red += val_Red * array_gy[r, c];
//                }
//            }
//            // finding Gx and Gy - attempt 2
//            // finding Gx and Gy - attempt 2





//            Gxy_sum_final_Blue = (int)Math.Sqrt((Gx_sum_Blue ^ 2) + (Gx_sum_Blue ^ 2));
//            Gxy_sum_final_Green = (int)Math.Sqrt((Gx_sum_Green ^ 2) + (Gx_sum_Green ^ 2));
//            Gxy_sum_final_Red = (int)Math.Sqrt((Gy_sum_Red ^ 2) + (Gy_sum_Red ^ 2));


//            //Gxy_sum_final_Blue *= 5;
//            //Gxy_sum_final_Green *= 5;
//            //Gxy_sum_final_Red *= 5;


//            if (Gxy_sum_final_Blue > 255)
//            {
//                Gxy_sum_final_Blue = 255;
//            }
//            if (Gxy_sum_final_Green > 255)
//            {
//                Gxy_sum_final_Green = 255;
//            }
//            if (Gxy_sum_final_Red > 255)
//            {
//                Gxy_sum_final_Red = 255;
//            }

//            if (Gxy_sum_final_Blue < 0)
//            {
//                Gxy_sum_final_Blue = 0;
//            }
//            if (Gxy_sum_final_Green < 0)
//            {
//                Gxy_sum_final_Green = 0;
//            }
//            if (Gxy_sum_final_Red < 0)
//            {
//                Gxy_sum_final_Red = 0;
//            }

//            //INVERT
//            Gxy_sum_final_Red = 255 - Gxy_sum_final_Red;
//            Gxy_sum_final_Green = 255 - Gxy_sum_final_Green;
//            Gxy_sum_final_Blue = 255 - Gxy_sum_final_Blue;
//            Gxy_sum_final_Red = (Gxy_sum_final_Red + Gxy_sum_final_Green + Gxy_sum_final_Blue) / 3; //
//            Gxy_sum_final_Green = (Gxy_sum_final_Red + Gxy_sum_final_Green + Gxy_sum_final_Blue) / 3;//
//            Gxy_sum_final_Blue = (Gxy_sum_final_Red + Gxy_sum_final_Green + Gxy_sum_final_Blue) / 3;//




//            val_Alpha = System.Drawing.Color.FromArgb(rgbArray4_wh_original[H, W]).A;

//            ////make alpha=0 background
//            //if (Gxy_sum_final_Blue >= 250 && Gxy_sum_final_Blue >= 250 && Gxy_sum_final_Blue >= 250)
//            //{
//            //    val_Alpha = 0;
//            //}



//            rgbArray4_wh[H, W] = (val_Alpha << 24) +
//                        (Gxy_sum_final_Red << 16) +
//                        (Gxy_sum_final_Green << 8) +
//                        (Gxy_sum_final_Blue);
//        }
//    }
//    //editing
//    //editing
//    //edit - apply ED
//    //edit - apply ED





//    //make array[] from array[,]
//    int counter = 0;
//    for (int r = 0; r < height; r++)
//    {
//        for (int c = 0; c < width; c++)
//        {
//            rgbArray4[counter] = rgbArray4_wh[r, c];
//            counter++;
//        }
//    }



//    Debug.WriteLine("check rgbArray4");
//    for (int i = 0; i < 4; i++)
//        Debug.WriteLine(System.Drawing.Color.FromArgb(rgbArray4[i]));
//    Debug.WriteLine("check rgbArray4_wh");
//    for (int i = 0; i < 1; i++)
//        for (int j = 0; j < 4; j++)
//            Debug.WriteLine(System.Drawing.Color.FromArgb(rgbArray4_wh[i, j]));
//    Debug.WriteLine("check rgbArray4_wh_original");
//    for (int i = 0; i < 1; i++)
//        for (int j = 0; j < 4; j++)
//            Debug.WriteLine(System.Drawing.Color.FromArgb(rgbArray4_wh_original[i, j]));



//    //apply edited rgbArray4 to wbitmap
//    var rect1 = new Int32Rect(0, 0, width, height);
//    wbitmap.WritePixels(rect1, rgbArray4, stride, 0);


//    //insert new wbitmap
//    imgPhoto.Source = wbitmap;
//    currentimage2 = wbitmap;

//}


////----------------------
////----------------------


////WIP, able to manipulate writeablebitmapimage, 
//// try rgbArray[,] in case u need it for edge detection algorithm
//// V5
//// should make it faster and more efficient (might need to centralize the rgb array, etc)
//private void Click_grayscale(object sender, RoutedEventArgs e)
//{

//    ////attempt 11
//    ////attempt 11 

//    WriteableBitmap wbitmap = new WriteableBitmap(currentimage2);


//    tb1.Text = wbitmap.PixelHeight.ToString() + " " + wbitmap.PixelWidth.ToString();
//    Trace.WriteLine(wbitmap.Height.ToString() + " " + wbitmap.Width.ToString());
//    Trace.WriteLine(wbitmap.PixelHeight.ToString() + " " + wbitmap.PixelWidth.ToString() + " " + wbitmap.BackBufferStride.ToString() + " " + wbitmap.Format.BitsPerPixel.ToString());


//    //initialize array
//    int[] rgbArray4 = new int[wbitmap.PixelHeight * wbitmap.PixelWidth]; //have to use pixelwidth when using writeablebitmap, idk why
//    int[,] rgbArray4_wh = new int[wbitmap.PixelHeight, wbitmap.PixelWidth]; //have to use pixelwidth when using writeablebitmap, idk why



//    //check rgbArray_getRGB4
//    for (int i = 0; i < 4; i++)
//        Debug.WriteLine(System.Drawing.Color.FromArgb(rgbArray4[i]));
//    //check rgbArray_getRGB4_wh
//    //to do


//    //info dimensions
//    int width = wbitmap.PixelWidth;
//    int height = wbitmap.PixelHeight;
//    int stride = wbitmap.BackBufferStride; //w * 4
//                                           ////int bytesPerPixel = (wbitmap.Format.BitsPerPixel + 7) / 8; - not needed?


//    //fill in rgbArray4
//    wbitmap.CopyPixels(rgbArray4, stride, 0);



//    //make array[,] out of data from array[]
//    int row_counter = 0;
//    int col_counter = 0;
//    for (int i = 0; i < width * height; i++)
//    {
//        rgbArray4_wh[row_counter, col_counter] = rgbArray4[i];
//        col_counter++;

//        // ex: row 0, col {ending number} to row 1, col 0
//        if (col_counter > width - 1)
//        {
//            col_counter = 0;
//            row_counter++;
//        }
//    }

//    //check rgbArray4 = ok
//    Debug.WriteLine("rgbArray4");
//    for (int i = 0; i < 4; i++)
//        Debug.WriteLine(System.Drawing.Color.FromArgb(rgbArray4[i]));
//    Debug.WriteLine("a " + System.Drawing.Color.FromArgb(rgbArray4[0])); //row 0, col 0
//    Debug.WriteLine("a " + System.Drawing.Color.FromArgb(rgbArray4[width - 1])); //row 0, col {end}
//    Debug.WriteLine("a " + System.Drawing.Color.FromArgb(rgbArray4[width])); //row 1, col 0
//    Debug.WriteLine("a " + System.Drawing.Color.FromArgb(rgbArray4[width * 2 - 1])); //row 1, col {end}


//    //check rgbArray_getRGB4_wh = ok
//    Debug.WriteLine("rgbArray4_wh");
//    for (int r = 0; r < 1; r++)
//        for (int c = 0; c < 4; c++)
//            Debug.WriteLine(System.Drawing.Color.FromArgb(rgbArray4_wh[r, c]));
//    Debug.WriteLine("wh " + System.Drawing.Color.FromArgb(rgbArray4_wh[0, 0])); //row 0, col 0
//    Debug.WriteLine("wh " + System.Drawing.Color.FromArgb(rgbArray4_wh[0, width - 1]));  //row 0, col {end}
//    Debug.WriteLine("wh " + System.Drawing.Color.FromArgb(rgbArray4_wh[1, 0])); //row 1, col 0
//    Debug.WriteLine("wh " + System.Drawing.Color.FromArgb(rgbArray4_wh[1, width - 1]));  //row 1, col {end}



//    ////byte[] pixelData = new Byte[stride]; //aka (width * 4) - not needed?





//    ////edit rgbArray4 - might need to be dropped in favor of rgbArray4_wh
//    //for (int i = 0; i < width * height; i++)
//    //{
//    //    //get color info in integar form
//    //    int alpha = System.Drawing.Color.FromArgb(rgbArray4[i]).A;
//    //    int red = System.Drawing.Color.FromArgb(rgbArray4[i]).R;
//    //    int green = System.Drawing.Color.FromArgb(rgbArray4[i]).G;
//    //    int blue = System.Drawing.Color.FromArgb(rgbArray4[i]).B;

//    //    //adjustments
//    //    alpha -= 20;
//    //    red -= 20;
//    //    green -= 20;
//    //    blue-= 50;

//    //    //keep ARGB within 0-255 range
//    //    if (alpha > 255) { alpha = 255; }
//    //    else if (alpha < 0) { alpha = 0; }
//    //    if (red > 255) { red = 255; }
//    //    else if (red < 0) { red = 0; }
//    //    if (green > 255) { green = 255; }
//    //    else if (green < 0) { green = 0; }
//    //    if (blue > 255) { blue = 255; }
//    //    else if (blue < 0) { blue = 0; }

//    //    //rgbArray4[i] = 259812; - not needed
//    //    // mylogic
//    //    // example: (255 << 24) A +    // A = 255
//    //    //          (100 << 16) R +    // R = 100
//    //    //          (200 << 16) R +    // G = 200
//    //    //          (255)              // B = 255 
//    //    rgbArray4[i] = (alpha << 24) +
//    //                   (red << 16) +
//    //                   (green << 8) +
//    //                   (blue);
//    //}



//    //edit rgbArray4_wh
//    for (int r = 0; r < height; r++)
//    {
//        for (int c = 0; c < width; c++)
//        {
//            //get color info in integar form
//            int alpha = System.Drawing.Color.FromArgb(rgbArray4_wh[r, c]).A;
//            int red = System.Drawing.Color.FromArgb(rgbArray4_wh[r, c]).R;
//            int green = System.Drawing.Color.FromArgb(rgbArray4_wh[r, c]).G;
//            int blue = System.Drawing.Color.FromArgb(rgbArray4_wh[r, c]).B;

//            //adjustments
//            alpha -= 10;
//            red -= 20;
//            green -= 20;
//            blue += 4;

//            //keep ARGB within 0-255 range
//            if (alpha > 255) { alpha = 255; }
//            else if (alpha < 0) { alpha = 0; }
//            if (red > 255) { red = 255; }
//            else if (red < 0) { red = 0; }
//            if (green > 255) { green = 255; }
//            else if (green < 0) { green = 0; }
//            if (blue > 255) { blue = 255; }
//            else if (blue < 0) { blue = 0; }

//            //rgbArray4[i] = 259812; - not needed
//            // mylogic
//            // example: (255 << 24) A +    // A = 255
//            //          (100 << 16) R +    // R = 100
//            //          (200 << 16) R +    // G = 200
//            //          (255)              // B = 255 
//            rgbArray4_wh[r, c] = (alpha << 24) +
//                           (red << 16) +
//                           (green << 8) +
//                           (blue);
//        }

//    }


//    //make array[] from array[,]
//    int counter = 0;
//    for (int r = 0; r < height; r++)
//    {
//        for (int c = 0; c < width; c++)
//        {
//            rgbArray4[counter] = rgbArray4_wh[r, c];
//            counter++;
//        }
//    }



//    //apply edited rgbArray4 to wbitmap
//    var rect1 = new Int32Rect(0, 0, width, height);
//    wbitmap.WritePixels(rect1, rgbArray4, stride, 0);


//    //insert new wbitmap
//    imgPhoto.Source = wbitmap;
//    currentimage2 = wbitmap;


//    ////attempt 11
//    ////attempt 11 
//}
////-----------------------------------
////-----------------------------------

// NOT WORKING
//private void Click_grayscale(object sender, RoutedEventArgs e)
//{

//    //attempt 1
//    //atempt1 1
//    //Bitmap bam = BitmapImage2Bitmap(currentimage);
//    //BitmapData bmpdata = bam.LockBits(new Rectangle(0, 0, bam.Width, bam.Height), ImageLockMode.ReadOnly, bam.PixelFormat);


//    //int numbytes = bmpdata.Stride * bam.Height;
//    //byte[] bytedata = new byte[bmpdata.Stride * bam.Height];


//    //IntPtr ptr = bmpdata.Scan0;
//    //Marshal.Copy(ptr, bytedata, 0, numbytes);

//    //if (bmpdata != null)
//    //    bam.UnlockBits(bmpdata);



//    //for (int i =0;i<14;i++)
//    //{
//    //    Debug.WriteLine(System.Drawing.Color.FromArgb(bytedata[i]));
//    //}
//    //attempt 1
//    //atempt1 1



//    //attempt 2
//    //attempt 2
//    //WriteableBitmap bmi2 = new WriteableBitmap(currentimage);


//    //System.Windows.Media.Color color = bmi2.GetPixel(30, 43);
//    //for (int i = 0; i < bmi2.Height; i++)

//    //    for (int j = 0; j < bmi2.Width; j++)

//    //        Debug.WriteLine(bmi2.GetPixel(i, j).A + " " + bmi2.GetPixel(i, j).R + " " + bmi2.GetPixel(i, j).B + " " + bmi2.GetPixel(i, j).G + i.ToString() + "|" + j.ToString());




//    //for (int pixel = 0; pixel < wbmi.Pixels.Length; pixel++)
//    //{
//    //    byte[] colorArray = BitConverter.GetBytes(wbmi.Pixels[pixel]);
//    //    byte blue = colorArray[0];
//    //    byte green = colorArray[1];
//    //    byte red = colorArray[2];
//    //    byte alpha = colorArray[3];

//    //}


//    //attempt 2
//    //attempt 2


//    ////attempt 3
//    ////attempt 3
//    //WriteableBitmap bmi2 = new WriteableBitmap(currentimage);

//    //bmi2.Lock();
//    //Debug.WriteLine("locked " + bmi2.Height + " " + bmi2.Width);
//    //Debug.WriteLine("locked " + bmi2.PixelHeight + " " + bmi2.PixelWidth);

//    ////bad array? doesn't print argb colors accurately
//    ////byte[] array2 = bmi2.ToByteArray();
//    ////for (int i = 0; i < 8; i++)
//    ////{ 
//    ////    //Debug.WriteLine(array2[i]);
//    ////    Debug.WriteLine(System.Drawing.Color.FromArgb(array2[i]));
//    ////}


//    //bmi2.Unlock();
//    //Debug.WriteLine("unlocked");

//    ////attempt 3
//    ////attempt 3




//    ////attempt 4
//    ////attempt 4 - slow method?, grayscale works seemly

//    //WriteableBitmap Bmp = new WriteableBitmap(currentimage);
//    //int rgb;
//    //System.Windows.Media.Color c;

//    //for (int y = 0; y < Bmp.Height; y++)

//    //    for (int x = 0; x < Bmp.Width; x++)
//    //    {
//    //        c = Bmp.GetPixel(x, y);
//    //        rgb = (int)((c.R + c.G + c.B) / 3);
//    //        Bmp.SetPixel(x, y, System.Windows.Media.Color.FromArgb(255, (byte)rgb, (byte)rgb, (byte)rgb)); //


//    //    }
//    //imgPhoto.Source = Bmp;

//    ////attempt 4
//    ////attempt 4




//    //attempt 5
//    //attempt 5 //rgb array (attempt) - seems to work (building an array of (A)RGB)

//    //BitmapImage bmi1 = currentimage;
//    //Bitmap bmi1_2_bm1 = BitmapImage2Bitmap(bmi1);
//    //int bwidth = bmi1_2_bm1.Width;
//    //int bheight = bmi1_2_bm1.Height;


//    ////log print
//    //tb1.Text = bwidth.ToString() + " " + bheight.ToString();
//    //Trace.WriteLine(bwidth.ToString() + " " + bheight.ToString()); //look for debug window, (not immediate window or console window)

//    //int numl = bwidth * bheight;
//    //int[] rgbArray = new int[numl];
//    ////getRGB(bmi1_2_bm1, 1, 1, 10, 10, rgbArray, 0, 10);
//    //getRGB(bmi1_2_bm1, 0, 0, bwidth, bheight, rgbArray, 0, bwidth); //working


//    //for (int i = 0; i < 4; i++)
//    //{
//    //    System.Drawing.Color colorinfo = System.Drawing.Color.FromArgb(rgbArray[i]);

//    //    Debug.Write(colorinfo + " " + rgbArray[i] + "\n");

//    //    rgbArray[i] = 0;

//    //}

//    //attempt 5
//    //attempt 5


//    ////attempt 6
//    ////attempt 6

//    //WriteableBitmap bmi4 = new WriteableBitmap(currentimage);
//    //var array3 = bmi4.ToByteArray(); // not good?
//    //for (int i = 0; i < 3; i ++)
//    //    Debug.WriteLine(System.Drawing.Color.FromArgb(array3[i]));

//    ////attempt 6
//    ////attempt 6


//    ////attempt 7
//    ////attempt 7 - border adder, WIP, need to fix protected memory error (when trying to modify certain photo)

//    //WriteableBitmap bmi5 = new WriteableBitmap(currentimage);

//    ////bmi5.Lock();
//    //for (int j = 0; j < bmi5.Height-1; j++)
//    //{
//    //    for (int i = 0; i < bmi5.Width; i++)
//    //    {

//    //        var pixel = bmi5.GetPixel(i, j); //doesn't seem to work w certain photo, causes protected memory error

//    //        if (i == 0 || i == bmi5.Width-1)
//    //        {
//    //            //var c = bmi5.GetPixel(i, j);
//    //            //c.R = 255;

//    //            bmi5.SetPixel(i, j, System.Windows.Media.Color.FromArgb(255, 255, 120, 120));

//    //            Debug.WriteLine(i + " " + pixel.A.ToString() + " " + pixel.R.ToString() + " " + pixel.G.ToString() + " " + pixel.B.ToString());

//    //        }
//    //    }
//    //}

//    //imgPhoto.Source = bmi5;
//    //Debug.WriteLine("done");


//    ////attempt 7
//    ////attempt 7




//    ////attempt 8
//    ////attempt 8 - maybe dont do byte array anymore?


//    //BitmapImage bma_bi = currentimage;
//    //Bitmap bma_b = BitmapImage2Bitmap(bma_bi);
//    //int bwidth = bma_b.Width; int bheight = bma_b.Height;

//    ////log print
//    //tb1.Text = bwidth.ToString() + " " + bheight.ToString();
//    //Debug.WriteLine(bwidth.ToString() + " " + bheight.ToString());

//    //byte[] array4 = Array1DFromBitmap(bma_b);
//    //for (int i = 0; i < 4; i++)
//    //{
//    //    Debug.WriteLine(System.Drawing.Color.FromArgb(array4[i]));
//    //}

//    ////attempt 8
//    ////attempt 8
//}




//private void Click_grayscale(object sender, RoutedEventArgs e)
//{



//////attempt 9
//////attempt 9 - SEEMS sucessful at making rgb array, and turn array into bitmap, should study this more.
/////

//BitmapImage bmi1 = currentimage;
//Bitmap bmi1_2_bm1 = BitmapImage2Bitmap(bmi1);
//int bwidth = bmi1_2_bm1.Width;
//int bheight = bmi1_2_bm1.Height;


////log print
//tb1.Text = bwidth.ToString() + " " + bheight.ToString();
//Trace.WriteLine(bwidth.ToString() + " " + bheight.ToString()); //look for debug window, (not immediate window or console window)

//int numl = bwidth * bheight;
//int[] rgbArray = new int[numl];
//int[,] rgbArray_wh = new int[bmi1_2_bm1.Height, bmi1_2_bm1.Width];


//Debug.WriteLine("testing getRGB 1 2 3");
////getRGB(bmi1_2_bm1, 1, 1, 10, 10, rgbArray, 0, 10);
////getRGB(bmi1_2_bm1, 0, 0, bwidth, bheight, rgbArray, 0, bwidth); //working
//getRGB2(bmi1_2_bm1, rgbArray); //working)
//getRGB3(bmi1_2_bm1, rgbArray_wh);



//Debug.WriteLine("testing rgbArray");
//for (int i = 0; i < 4; i++)
//{
//    System.Drawing.Color colorinfo = System.Drawing.Color.FromArgb(rgbArray[i]);
//    Debug.Write(colorinfo + " " + rgbArray[i] + "\n");
//}
//Debug.WriteLine("testing rgbArray_wh"); //seems ok
//for (int r = 0; r < 1; r++)
//    for (int c = 0; c < 4; c++)
//    {
//        System.Drawing.Color colorinfo = System.Drawing.Color.FromArgb(rgbArray_wh[r,c]);
//        Debug.Write(colorinfo + " " + rgbArray_wh[r,c] + "\n");
//    }



////how to turn rgb array into bitmap //abandoned
////byte[] rgbArraybb = new byte[rgbArray.Length];
////for (int i = 0; i < rgbArray.Length; i++)
////{
////    rgbArraybb[i] = (byte)rgbArray[i];
////}
////Debug.WriteLine("fad");
////for (int i = 0; i < 6; i++)
////{
////    Debug.WriteLine(rgbArraybb[i]);
////}



//////make bitmap from array attempt //abanconed
////bmw.SetPixel(0, 0, System.Drawing.Color.FromArgb(255, 12, 12, 12));





//////OK
////BitmapImage bmz = Bitmap2BitmapImage(bmi1_2_bm1);
////imgPhoto.Source = bmz;


////Bitmap bmal = InttoBitmap(rgbArray);
//Bitmap bmal = InttoBitmap2(bmi1_2_bm1, rgbArray_wh); // WIP
//BitmapImage bmzs = Bitmap2BitmapImage(bmal);
//imgPhoto.Source = bmzs;

//currentimage = bmzs;


//////attempt 9
//////attempt 9
///



//// WIP - issue (can't preserve ALPHA channel when using BitmapImage2Bitmap, Bitmap2BitmapImage)
////functions used: getrgb3, BitmapImage2Bitmap, Bitmap2BitmapImage, InttoBitmap2
//private void Click_grayscale(object sender, RoutedEventArgs e)
//{

//    ////attempt 10
//    ////attempt 10 - SEEMS sucessful at making rgb array, and turn array into bitmap, should study this more.
//    ///

//    //retrieve current bitmapimage, retrieve bitmap, log print
//    BitmapImage bi = currentimage;
//    Bitmap bmp = BitmapImage2Bitmap(bi);
//    tb1.Text = bmp.Width.ToString() + " " + bmp.Height.ToString();
//    Trace.WriteLine(bmp.Width.ToString() + " " + bmp.Height.ToString());


//    //make empty array of bitmap
//    int[,] rgbArray_wh = new int[bmp.Height, bmp.Width];


//    //fill-in empty array (should be for loadapic?)
//    getRGB3(bmp, rgbArray_wh);

//    //
//    Debug.WriteLine("checking rgbArray_wh");
//    for (int r = 0; r < 1; r++)
//        for (int c = 0; c < 4; c++)
//        {
//            System.Drawing.Color colorinfo = System.Drawing.Color.FromArgb(rgbArray_wh[r, c]);
//            Debug.Write(colorinfo + " | " + rgbArray_wh[r, c] + "\n");
//        }


//    //edit array -  coming soon?
//    editRGB(bmp, rgbArray_wh);

//    //turn array into bitmap, turn bitmap into bitmapimage (for imgPhoto.Source and currentimage)
//    Bitmap bmp_new = InttoBitmap2(bmp, rgbArray_wh); // WIP
//    BitmapImage bi_new = Bitmap2BitmapImage(bmp_new);
//    imgPhoto.Source = bi_new;
//    currentimage = bi_new;


//    ////attempt 10
//    ////attempt 10
//}



//functions used: getrgb3, BitmapImage2Bitmap, Bitmap2BitmapImage, InttoBitmap2
//




////WIP, able to manipulate writeablebitmapimage, 
//// try rgbArray[,] in case u need it for edge detection algorithm
////need to set currentimage2 so you can apply edit over and over (SOLVED)
////need to figure out how to set new color values on rgbarray[i] by color (SOLVED)
//// better name: click_edit
//// V4
//private void Click_grayscale(object sender, RoutedEventArgs e)
//{

//    ////attempt 11
//    ////attempt 11 


//    //retrieve current bitmapimage, retrieve bitmap, log print
//    WriteableBitmap wbitmap = new WriteableBitmap(currentimage2);
//    tb1.Text = wbitmap.PixelWidth.ToString() + " " + wbitmap.PixelHeight.ToString();
//    Trace.WriteLine(wbitmap.Width.ToString() + " " + wbitmap.Height.ToString());
//    Trace.WriteLine(wbitmap.PixelWidth.ToString() + " " + wbitmap.PixelHeight.ToString() + " " + wbitmap.BackBufferStride.ToString() + " " + wbitmap.Format.BitsPerPixel.ToString());


//    //initialize array (might notbe needed for writeablebitmap)
//    int[] rgbArray4 = new int[wbitmap.PixelHeight * wbitmap.PixelWidth]; //have to use pixelwidth when using writeablebitmap, idk why
//    int[,] rgbArray4_wh = new int[wbitmap.PixelHeight, wbitmap.PixelWidth]; //have to use pixelwidth when using writeablebitmap, idk why

//    //check rgbArray_getRGB4
//    for (int i = 0; i < 4; i++)
//        Debug.WriteLine(System.Drawing.Color.FromArgb(rgbArray4[i]));
//    //check rgbArray_getRGB4_wh
//    //to do


//    //info dimensions
//    int width = wbitmap.PixelWidth;
//    int height = wbitmap.PixelHeight;
//    int stride = wbitmap.BackBufferStride; //w * 4
//                                           ////int bytesPerPixel = (wbitmap.Format.BitsPerPixel + 7) / 8; - not needed?


//    //fill in rgbArray4
//    wbitmap.CopyPixels(rgbArray4, stride, 0);


//    //check rgbArray4
//    for (int i = 0; i < 4; i++)
//        Debug.WriteLine(System.Drawing.Color.FromArgb(rgbArray4[i]));
//    //check rgbArray_getRGB4_wh
//    //to do



//    ////byte[] pixelData = new Byte[stride]; //aka (width * 4) - not needed?


//    //edit rgbArray4
//    for (int i = 0; i < width * height; i++)
//    {
//        //get color info in integar form
//        int alpha = System.Drawing.Color.FromArgb(rgbArray4[i]).A;
//        int red = System.Drawing.Color.FromArgb(rgbArray4[i]).R;
//        int green = System.Drawing.Color.FromArgb(rgbArray4[i]).G;
//        int blue = System.Drawing.Color.FromArgb(rgbArray4[i]).B;

//        //adjustments
//        alpha -= 20;
//        red -= 20;
//        green -= 20;
//        blue -= 50;

//        //keep ARGB within 0-255 range
//        if (alpha > 255) { alpha = 255; }
//        else if (alpha < 0) { alpha = 0; }
//        if (red > 255) { red = 255; }
//        else if (red < 0) { red = 0; }
//        if (green > 255) { green = 255; }
//        else if (green < 0) { green = 0; }
//        if (blue > 255) { blue = 255; }
//        else if (blue < 0) { blue = 0; }

//        //rgbArray4[i] = 259812; - not needed
//        // mylogic
//        // example: (255 << 24) A +    // A = 255
//        //          (100 << 16) R +    // R = 100
//        //          (200 << 16) R +    // G = 200
//        //          (255)              // B = 255 
//        rgbArray4[i] = (alpha << 24) +
//                       (red << 16) +
//                       (green << 8) +
//                       (blue);
//    }

//    //apply edited rgbArray4 to wbitmap
//    var rect1 = new Int32Rect(0, 0, width, height);
//    wbitmap.WritePixels(rect1, rgbArray4, stride, 0);


//    //insert new wbitmap
//    imgPhoto.Source = wbitmap;
//    currentimage2 = wbitmap;


//    ////attempt 11
//    ////attempt 11 
//}







//////OLD, DONT DELETE
//////seems to work - Explore this! testing currently, v1, 
//////try int[,] instead of int[]
//private Bitmap InttoBitmap(int[] blob)
//{
//    int w = 400;
//    int h = 549;
//    Bitmap bm = new Bitmap(w, h, PixelFormat.Format32bppArgb);



//    int i = 0;
//    Debug.WriteLine("inttobitmap1");
//    for (int col = 0; col < w; col++)
//    {
//        for (int row = 0; row < h; row++)
//        {
//            //System.Drawing.Color color = new System.Drawing.Color();
//            var color = System.Drawing.Color.FromArgb(blob[i]);

//            if (col < 1 && row < 4)
//                Debug.WriteLine(color.A.ToString() + " " + ((int)color.R).ToString() + " " + color.G.ToString() + " " + color.B.ToString());


//            //bm.SetPixel(col, row, color);
//            //bm.SetPixel(col, row, System.Drawing.Color.FromArgb(255, 0, 0, 120));
//            bm.SetPixel(col, row, System.Drawing.Color.FromArgb((int)color.A, (int)color.R, (int)color.G, (int)color.B));
//            i++;
//        }

//    }
//    return bm;
//}

//BitmapImage bitmapSource;
//using(MemoryStream outStream = new MemoryStream())
//{
//    BitmapEncoder enc = new BmpBitmapEncoder();
//    enc.Frames.Add(BitmapFrame.Create(bitmapSource));
//    enc.Save(outStream);
//    System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);
//}








////OLD
//private static void getRGB2(Bitmap bmp, int[] rgbArray)
//{
//    Debug.WriteLine("testing RGB2");
//    const int PixelWidth = 3;
//    //const int offset = 0;
//    int scansize = bmp.Width;

//    BitmapData bmpdata = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);

//    try
//    {

//        byte[] pixelData = new Byte[bmpdata.Stride]; //aka (width * 4)
//        Debug.WriteLine("stride (width * 4):" + bmpdata.Stride.ToString() + " | width:" + bmpdata.Width.ToString());


//        for (int row = 0; row < bmpdata.Height; row++)

//        {
//            Marshal.Copy(bmpdata.Scan0 + (row * bmpdata.Stride), pixelData, 0, bmpdata.Stride);

//            for (int col = 0; col < bmpdata.Width; col++)
//            {
//                // PixelFormat.Format32bppRgb means the data is stored
//                // in memory as BGR. We want RGB, so we must do some 
//                // bit-shuffling.
//                //rgbArray[offset + (row * scansize) + col] =
//                //    (pixelData[col * PixelWidth + 2] << 16) +   // R 
//                //    (pixelData[col * PixelWidth + 1] << 8) +    // G
//                //    pixelData[col * PixelWidth];                // 
//                //rgbArray[(row * scansize) + col] =
//                //    (pixelData[col * PixelWidth + 2] << 16) +   // R 
//                //    (pixelData[col * PixelWidth + 1] << 8) +    // G
//                //    pixelData[col * PixelWidth];                // B
//                rgbArray[(row * scansize) + col] =
//                    (pixelData[col * PixelWidth + 3] << 24) +
//                    (pixelData[col * PixelWidth + 2] << 16) +   // R 
//                    (pixelData[col * PixelWidth + 1] << 8) +    // G
//                    pixelData[col * PixelWidth];                // B

//                if (row < 1 && col < 4)
//                {
//                    Debug.WriteLine(pixelData[col * PixelWidth + 3].ToString() + " " + pixelData[col * PixelWidth + 2].ToString() + " " + pixelData[col * PixelWidth + 1].ToString() + " " + pixelData[col * PixelWidth].ToString());
//                    //seems to print B G R
//                    // 00 00 00
//                    // 120 200 255 (bgr)
//                    // 

//                    //Debug.WriteLine(pixelData[col * PixelWidth + 2] + pixelData[col * PixelWidth + 1] + pixelData[col * PixelWidth]);
//                    //Debug.WriteLine((pixelData[col * PixelWidth + 3] << 32));
//                    //Debug.WriteLine((pixelData[col * PixelWidth + 2] << 16));
//                    //Debug.WriteLine((pixelData[col * PixelWidth + 1] << 8));
//                    //Debug.WriteLine((pixelData[col * PixelWidth]));
//                    //Debug.WriteLine("");
//                    //(pixelData[col * PixelWidth + 1] << 8) +
//                    //+ pixelData[col * PixelWidth]

//                    //try argb struct?
//                }
//            }
//        }
//    }

//    finally
//    {
//        bmp.UnlockBits(bmpdata);
//    }
//}





////getRGB(bmi1_2_bm1, 1, 1, 10, 10, rgbArray, 0, 10);
////OLD, WORKING
//private static void getRGB(Bitmap image, int startX, int startY, int w, int h, int[] rgbArray, int offset, int scansize)
//{
//    const int PixelWidth = 3;
//    if (image == null) throw new ArgumentNullException("image");
//    if (rgbArray == null) throw new ArgumentNullException("rgbArray");
//    if (startX < 0 || startX + w > image.Width) throw new ArgumentOutOfRangeException("startX");
//    if (startY < 0 || startY + h > image.Height) throw new ArgumentOutOfRangeException("startY");
//    if (w < 0 || w > scansize || w > image.Width) throw new ArgumentOutOfRangeException("w");
//    if (h < 0 || (rgbArray.Length < offset + h * scansize) || h > image.Height) throw new ArgumentOutOfRangeException("h");

//    BitmapData data = image.LockBits(new Rectangle(startX, startY, w, h), System.Drawing.Imaging.ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
//    ////example
//    //BitmapData data = bmp.LockBits(new Rectangle(0, 0, 640, 480), ImageLockMode.ReadOnly,  System.Drawing.Imaging.PixelFormat.Format32bppArgb);

//    try
//    {
//        byte[] pixelData = new Byte[data.Stride];
//        for (int scanline = 0; scanline < data.Height; scanline++)
//        {
//            Marshal.Copy(data.Scan0 + (scanline * data.Stride), pixelData, 0, data.Stride);
//            for (int pixeloffset = 0; pixeloffset < data.Width; pixeloffset++)
//            {
//                // PixelFormat.Format32bppRgb means the data is stored
//                // in memory as BGR. We want RGB, so we must do some 
//                // bit-shuffling.
//                rgbArray[offset + (scanline * scansize) + pixeloffset] =
//                    (pixelData[pixeloffset * PixelWidth + 2] << 16) +   // R 
//                    (pixelData[pixeloffset * PixelWidth + 1] << 8) +    // G
//                    pixelData[pixeloffset * PixelWidth];                // B
//            }
//        }
//    }
//    finally
//    {
//        image.UnlockBits(data);
//    }
//}






////WORKS fine for bitmap, but need to try writeablebitmap (getRGB4)
////making getRGB3 for int[,] array to see if int[,] is needed for bitmap manipulation
//// not needed
//private static void getRGB3(Bitmap bmp, int[,] rgbArray_wh)
//{
//    Debug.WriteLine("testing RGB3");
//    const int PixelWidth = 4; //3 for rgb? 4 FOR ARGB?
//    //const int offset = 0;
//    //int scansize = bmp.Width;

//    BitmapData bmpdata = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);

//    try
//    {

//        byte[] pixelData = new Byte[bmpdata.Stride]; //aka (width * 4)
//        Debug.WriteLine("stride (width * 4):" + bmpdata.Stride.ToString() + " | width:" + bmpdata.Width.ToString());


//        for (int row = 0; row < bmpdata.Height; row++)

//        {
//            Marshal.Copy(bmpdata.Scan0 + (row * bmpdata.Stride), pixelData, 0, bmpdata.Stride);

//            for (int col = 0; col < bmpdata.Width; col++)
//            {
//                rgbArray_wh[row, col] =
//                    (pixelData[col * PixelWidth + 3] << 24) +   // A?
//                    (pixelData[col * PixelWidth + 2] << 16) +   // R 
//                    (pixelData[col * PixelWidth + 1] << 8) +    // G
//                    pixelData[col * PixelWidth];                // B


//                if (row < 1 && col < 4)
//                {
//                    Debug.WriteLine(System.Drawing.Color.FromArgb(rgbArray_wh[row, col]).ToString() + " | " + rgbArray_wh[row, col].ToString());
//                }

//            }


//        }

//    }

//    finally
//    {
//        bmp.UnlockBits(bmpdata);
//    }
//}
