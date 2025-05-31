using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using LibraryDesktop.Models;

namespace LibraryDesktop.View
{
    public partial class BookControl : UserControl
    {
        private LibraryDesktop.Models.Book? _book;
        public event EventHandler<LibraryDesktop.Models.Book>? BookClicked;

        // Parameterless constructor for designer support
        public BookControl()
        {
            InitializeComponent();
        }

        public BookControl(LibraryDesktop.Models.Book book)
        {
            _book = book;
            InitializeComponent();
            PopulateData();
        }

        public void SetBook(LibraryDesktop.Models.Book book)
        {
            _book = book;
            PopulateData();
        }
          /// <summary>
        /// Refresh the chapter count display - useful when TotalChapters has been updated
        /// </summary>
        public void RefreshChapterCount()
        {
            if (_book != null)
            {
                // Improved chapter count calculation with better fallback logic
                int chapterCount = 0;
                if (_book.TotalChapters > 0)
                {
                    chapterCount = _book.TotalChapters;
                }
                else if (_book.Chapters != null && _book.Chapters.Count > 0)
                {
                    chapterCount = _book.Chapters.Count;
                }
                
                lblChapterCount.Text = chapterCount > 0 ? $"{chapterCount} Chapters" : "No chapters";
                
                System.Diagnostics.Debug.WriteLine($"📖 Refreshed chapter count for BookId {_book.BookId}: {chapterCount}");
            }
        }
          private void PopulateData()
        {
            if (_book != null)
            {
                // Truncate title if too long to prevent overflow
                string displayTitle = _book.Title;
                if (displayTitle.Length > 25)
                {
                    displayTitle = displayTitle.Substring(0, 22) + "...";
                }
                lblBookTitle.Text = displayTitle;

                lblAuthor.Text = $"{_book.Author}";
                
                // Improved chapter count calculation with better fallback logic
                int chapterCount = 0;
                if (_book.TotalChapters > 0)
                {
                    chapterCount = _book.TotalChapters;
                }
                else if (_book.Chapters != null && _book.Chapters.Count > 0)
                {
                    chapterCount = _book.Chapters.Count;
                }
                
                lblChapterCount.Text = chapterCount > 0 ? $"{chapterCount} Chapters" : "No chapters";

                // Show price panel if book has a price
                if (_book.Price > 0)
                {
                    panelPremium.Visible = true;
                    lblPrice.Text = $"{_book.Price} Coins";
                }
                else
                {
                    panelPremium.Visible = false;
                }

                // Load book cover image from Assets folder based on BookId
                LoadBookCoverImage();
            }
        }        private void LoadBookCoverImage()
        {
            if (_book != null)
            {
                try
                {
                    // Dispose previous image if it exists to prevent memory leaks
                    if (picBookCover.Image != null)
                    {
                        var oldImage = picBookCover.Image;
                        picBookCover.Image = null;
                        oldImage.Dispose();
                    }

                    // Don't resize the PictureBox - keep the designer dimensions
                    // This prevents layout issues with the title and other elements
                    int width = picBookCover.Width;
                    int height = picBookCover.Height;

                    string? imagePath = null;
                    bool imageFound = false;

                    // Debug: Log current directory and BookId
                    System.Diagnostics.Debug.WriteLine($"🔍 StartupPath: {Application.StartupPath}");
                    System.Diagnostics.Debug.WriteLine($"📖 Looking for image for BookId: {_book.BookId}");
                    System.Diagnostics.Debug.WriteLine($"🖼️ PictureBox size: {picBookCover.Width}x{picBookCover.Height}");

                    // Method 1: Try to load image based on BookId (e.g., 1.png, 2.png, etc.)
                    imagePath = Path.Combine(Application.StartupPath, "Assets", $"{_book.BookId}.png");
                    System.Diagnostics.Debug.WriteLine($"🎯 Trying primary path: {imagePath}");
                    
                    if (File.Exists(imagePath))
                    {
                        imageFound = true;
                        System.Diagnostics.Debug.WriteLine($"✅ Found image at: {imagePath}");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"❌ Image not found at: {imagePath}");
                        
                        // Method 2: If specific BookId image doesn't exist, try CoverImageUrl
                        if (!string.IsNullOrEmpty(_book.CoverImageUrl))
                        {
                            System.Diagnostics.Debug.WriteLine($"🔗 Trying CoverImageUrl: {_book.CoverImageUrl}");
                            
                            // Handle different path formats in CoverImageUrl
                            if (_book.CoverImageUrl.StartsWith("LibraryDesktop/Assets/", StringComparison.OrdinalIgnoreCase) ||
                                _book.CoverImageUrl.StartsWith("LibraryDesktop\\Assets\\", StringComparison.OrdinalIgnoreCase))
                            {
                                // Extract just the filename from the full path
                                var fileName = Path.GetFileName(_book.CoverImageUrl);
                                imagePath = Path.Combine(Application.StartupPath, "Assets", fileName);
                            }
                            else if (_book.CoverImageUrl.StartsWith("Assets/", StringComparison.OrdinalIgnoreCase) ||
                                     _book.CoverImageUrl.StartsWith("Assets\\", StringComparison.OrdinalIgnoreCase))
                            {
                                imagePath = Path.Combine(Application.StartupPath, _book.CoverImageUrl.Replace('/', Path.DirectorySeparatorChar));
                            }
                            else
                            {
                                // Assume it's just a filename
                                imagePath = Path.Combine(Application.StartupPath, "Assets", _book.CoverImageUrl);
                            }

                            System.Diagnostics.Debug.WriteLine($"🔗 Trying CoverImageUrl path: {imagePath}");
                            if (File.Exists(imagePath))
                            {
                                imageFound = true;
                                System.Diagnostics.Debug.WriteLine($"✅ Found image via CoverImageUrl at: {imagePath}");
                            }
                        }

                        // Method 3: Try different extensions for the BookId
                        if (!imageFound)
                        {
                            string[] extensions = { ".png", ".jpg", ".jpeg", ".gif", ".bmp" };
                            foreach (string ext in extensions)
                            {
                                string fallbackPath = Path.Combine(Application.StartupPath, "Assets", $"{_book.BookId}{ext}");
                                System.Diagnostics.Debug.WriteLine($"🔄 Trying extension path: {fallbackPath}");
                                if (File.Exists(fallbackPath))
                                {
                                    imagePath = fallbackPath;
                                    imageFound = true;
                                    System.Diagnostics.Debug.WriteLine($"✅ Found image with extension at: {fallbackPath}");
                                    break;
                                }
                            }
                        }

                        // Method 4: Fallback to consistent existing image using modulo for same BookId
                        if (!imageFound)
                        {
                            var assetsDir = Path.Combine(Application.StartupPath, "Assets");
                            if (Directory.Exists(assetsDir))
                            {
                                var availableImages = Directory.GetFiles(assetsDir, "*.png")
                                    .Concat(Directory.GetFiles(assetsDir, "*.jpg"))
                                    .Concat(Directory.GetFiles(assetsDir, "*.jpeg"))
                                    .ToArray();

                                if (availableImages.Length > 0)
                                {
                                    // Use modulo to get a consistent image for the same BookId
                                    int index = _book.BookId % availableImages.Length;
                                    imagePath = availableImages[index];
                                    imageFound = true;
                                    System.Diagnostics.Debug.WriteLine($"🎲 Using fallback image: {imagePath}");
                                }
                            }
                        }
                    }                    // Load the image if found
                    if (imageFound && !string.IsNullOrEmpty(imagePath))
                    {
                        // Load image with fixed PictureBox dimensions to prevent layout issues
                        using (var fileStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                        {
                            var originalImage = Image.FromStream(fileStream);
                            
                            // Create a properly scaled image that fits the fixed PictureBox size
                            var scaledImage = new Bitmap(width, height);
                            using (var graphics = Graphics.FromImage(scaledImage))
                            {
                                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                                
                                // Draw image with proper scaling to fill PictureBox without changing its size
                                graphics.DrawImage(originalImage, 0, 0, width, height);
                            }
                            
                            picBookCover.Image = scaledImage;
                            originalImage.Dispose();
                        }
                        
                        picBookCover.SizeMode = PictureBoxSizeMode.StretchImage;
                        picBookCover.BackColor = Color.Transparent;
                        System.Diagnostics.Debug.WriteLine($"✅ Successfully loaded and scaled image for BookId {_book.BookId}: {imagePath}");
                    }
                    else
                    {
                        // Create a default image with fixed dimensions
                        CreateDefaultBookCover();
                        System.Diagnostics.Debug.WriteLine($"🎨 No image found for BookId {_book.BookId}, using default generated cover");
                    }
                }
                catch (Exception ex)
                {
                    // If image loading fails, set a fallback appearance
                    System.Diagnostics.Debug.WriteLine($"❌ Error loading book cover image for BookId {_book.BookId}: {ex.Message}");
                    CreateDefaultBookCover();
                }
            }
        }        private void CreateDefaultBookCover()
        {
            try
            {
                // Use the fixed PictureBox dimensions from the designer
                int width = picBookCover.Width;
                int height = picBookCover.Height;
                
                // Ensure we have valid dimensions
                if (width <= 0) width = 197; // Designer default
                if (height <= 0) height = 236; // Designer default
                
                var bitmap = new Bitmap(width, height);
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                    
                    // Create attractive gradient background
                    using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                        new Rectangle(0, 0, width, height),
                        Color.FromArgb(120, 160, 200),
                        Color.FromArgb(70, 110, 150),
                        System.Drawing.Drawing2D.LinearGradientMode.Vertical))
                    {
                        graphics.FillRectangle(brush, 0, 0, width, height);
                    }

                    // Add border
                    using (var borderPen = new Pen(Color.FromArgb(200, Color.White), 3))
                    {
                        graphics.DrawRectangle(borderPen, 2, 2, width - 5, height - 5);
                    }

                    // Add book icon with appropriate font size
                    int fontSize = Math.Max(24, width / 8); // Consistent sizing based on width
                    using (var font = new Font("Segoe UI", fontSize, FontStyle.Bold))
                    using (var textBrush = new SolidBrush(Color.White))
                    {
                        var text = "📚";
                        var textSize = graphics.MeasureString(text, font);
                        var x = (width - textSize.Width) / 2;
                        var y = (height - textSize.Height) / 2 - 10;
                        graphics.DrawString(text, font, textBrush, x, y);
                    }
                    
                    // Add "No Image" text below icon
                    using (var smallFont = new Font("Segoe UI", Math.Max(10, width / 18), FontStyle.Regular))
                    using (var textBrush = new SolidBrush(Color.FromArgb(240, 240, 240)))
                    {
                        var text = "No Image";
                        var textSize = graphics.MeasureString(text, smallFont);
                        var x = (width - textSize.Width) / 2;
                        var y = height * 0.72f;
                        graphics.DrawString(text, smallFont, textBrush, x, y);
                    }
                }

                picBookCover.Image = bitmap;
                picBookCover.SizeMode = PictureBoxSizeMode.StretchImage;
                picBookCover.BackColor = Color.Transparent;
                
                System.Diagnostics.Debug.WriteLine($"🎨 Created default book cover with fixed size: {width}x{height}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error creating default book cover: {ex.Message}");
                // Ultimate fallback
                picBookCover.BackColor = Color.FromArgb(200, 220, 240);
                picBookCover.SizeMode = PictureBoxSizeMode.CenterImage;
                picBookCover.Image = null;
            }
        }private void BookControl_Click(object sender, EventArgs e)
        {
            if (_book != null)
            {
                BookClicked?.Invoke(this, _book);
            }
        }
    }
}