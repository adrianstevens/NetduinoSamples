namespace WaveShare_EInk
{
    public class EpdPaint
    {
        // Display orientation
        readonly int ROTATE_0 = 0;
        readonly int ROTATE_90 = 1;
        readonly int ROTATE_180 = 2;
        readonly int ROTATE_270 = 3;

        // Color inverse. 1 or 0 = set or reset a bit if set a colored pixel
        int IF_INVERT_COLOR = 1;

        byte[] image;
        int width;
        int height;
        int rotate;

        void Paint(byte[] image, int width, int height)
        {
            this.rotate = ROTATE_0;
            this.image = image;
            /* 1 byte = 8 pixels, so the width should be the multiple of 8 */
            this.width = (width % 8 > 0) ? width + 8 - (width % 8) : width;
            this.height = height;
        }

        void Clear(int colored)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    DrawAbsolutePixel(x, y, colored);
                }
            }
        }

        void DrawAbsolutePixel(int x, int y, int colored)
        {
            if (x < 0 || x >= this.width || y < 0 || y >= this.height)
            {
                return;
            }
            if (IF_INVERT_COLOR > 0)
            {
                if (colored > 0)
                {
                    image[(x + y * this.width) / 8] |= (byte)(0x80 >> (x % 8));
                }
                else
                {
                    image[(x + y * this.width) / 8] &= (byte)~((0x80 >> (x % 8)));
                }
            }
            else
            {
                if (colored > 0)
                {
                    image[(x + y * this.width) / 8] &= (byte)~(0x80 >> (x % 8));
                }
                else
                {
                    image[(x + y * this.width) / 8] |= (byte)(0x80 >> (x % 8));
                }
            }
        }

        byte[] GetImage()
        {
            return this.image;
        }

        int GetWidth()
        {
            return this.width;
        }

        void SetWidth(int width)
        {
            this.width = (width % 8 > 0) ? width + 8 - (width % 8) : width;
        }

        int GetHeight()
        {
            return this.height;
        }

        void SetHeight(int height)
        {
            this.height = height;
        }

        int GetRotate()
        {
            return this.rotate;
        }

        void SetRotate(int rotate)
        {
            this.rotate = rotate;
        }

        /**
         *  @brief: this draws a pixel by the coordinates
         */
        void DrawPixel(int x, int y, int colored)
        {
            int point_temp;
            if (this.rotate == ROTATE_0)
            {
                if (x < 0 || x >= this.width || y < 0 || y >= this.height)
                {
                    return;
                }
                DrawAbsolutePixel(x, y, colored);
            }
            else if (this.rotate == ROTATE_90)
            {
                if (x < 0 || x >= this.height || y < 0 || y >= this.width)
                {
                    return;
                }
                point_temp = x;
                x = this.width - y;
                y = point_temp;
                DrawAbsolutePixel(x, y, colored);
            }
            else if (this.rotate == ROTATE_180)
            {
                if (x < 0 || x >= this.width || y < 0 || y >= this.height)
                {
                    return;
                }
                x = this.width - x;
                y = this.height - y;
                DrawAbsolutePixel(x, y, colored);
            }
            else if (this.rotate == ROTATE_270)
            {
                if (x < 0 || x >= this.height || y < 0 || y >= this.width)
                {
                    return;
                }
                point_temp = x;
                x = y;
                y = this.height - point_temp;
                DrawAbsolutePixel(x, y, colored);
            }
        }


        /**
        *  @brief: this draws a line on the frame buffer
        */
        void DrawLine(int x0, int y0, int x1, int y1, int colored)
        {
            /* Bresenham algorithm */
            int dx = x1 - x0 >= 0 ? x1 - x0 : x0 - x1;
            int sx = x0 < x1 ? 1 : -1;
            int dy = y1 - y0 <= 0 ? y1 - y0 : y0 - y1;
            int sy = y0 < y1 ? 1 : -1;
            int err = dx + dy;

            while ((x0 != x1) && (y0 != y1))
            {
                DrawPixel(x0, y0, colored);
                if (2 * err >= dy)
                {
                    err += dy;
                    x0 += sx;
                }
                if (2 * err <= dx)
                {
                    err += dx;
                    y0 += sy;
                }
            }
        }

        /**
        *  @brief: this draws a horizontal line on the frame buffer
        */
        void DrawHorizontalLine(int x, int y, int line_width, int colored)
        {
            int i;
            for (i = x; i < x + line_width; i++)
            {
                DrawPixel(i, y, colored);
            }
        }

        /**
        *  @brief: this draws a vertical line on the frame buffer
        */
        void DrawVerticalLine(int x, int y, int line_height, int colored)
        {
            int i;
            for (i = y; i < y + line_height; i++)
            {
                DrawPixel(x, i, colored);
            }
        }

        /**
        *  @brief: this draws a rectangle
        */
        void DrawRectangle(int x0, int y0, int x1, int y1, int colored)
        {
            int min_x, min_y, max_x, max_y;
            min_x = x1 > x0 ? x0 : x1;
            max_x = x1 > x0 ? x1 : x0;
            min_y = y1 > y0 ? y0 : y1;
            max_y = y1 > y0 ? y1 : y0;

            DrawHorizontalLine(min_x, min_y, max_x - min_x + 1, colored);
            DrawHorizontalLine(min_x, max_y, max_x - min_x + 1, colored);
            DrawVerticalLine(min_x, min_y, max_y - min_y + 1, colored);
            DrawVerticalLine(max_x, min_y, max_y - min_y + 1, colored);
        }

        /**
        *  @brief: this draws a filled rectangle
        */
        void DrawFilledRectangle(int x0, int y0, int x1, int y1, int colored)
        {
            int min_x, min_y, max_x, max_y;
            int i;
            min_x = x1 > x0 ? x0 : x1;
            max_x = x1 > x0 ? x1 : x0;
            min_y = y1 > y0 ? y0 : y1;
            max_y = y1 > y0 ? y1 : y0;

            for (i = min_x; i <= max_x; i++)
            {
                DrawVerticalLine(i, min_y, max_y - min_y + 1, colored);
            }
        }

        /**
        *  @brief: this draws a circle
        */
        void DrawCircle(int x, int y, int radius, int colored)
        {
            /* Bresenham algorithm */
            int x_pos = -radius;
            int y_pos = 0;
            int err = 2 - 2 * radius;
            int e2;

            do
            {
                DrawPixel(x - x_pos, y + y_pos, colored);
                DrawPixel(x + x_pos, y + y_pos, colored);
                DrawPixel(x + x_pos, y - y_pos, colored);
                DrawPixel(x - x_pos, y - y_pos, colored);
                e2 = err;
                if (e2 <= y_pos)
                {
                    err += ++y_pos * 2 + 1;
                    if (-x_pos == y_pos && e2 <= x_pos)
                    {
                        e2 = 0;
                    }
                }
                if (e2 > x_pos)
                {
                    err += ++x_pos * 2 + 1;
                }
            } while (x_pos <= 0);
        }

        /**
        *  @brief: this draws a filled circle
        */
        void DrawFilledCircle(int x, int y, int radius, int colored)
        {
            /* Bresenham algorithm */
            int x_pos = -radius;
            int y_pos = 0;
            int err = 2 - 2 * radius;
            int e2;

            do
            {
                DrawPixel(x - x_pos, y + y_pos, colored);
                DrawPixel(x + x_pos, y + y_pos, colored);
                DrawPixel(x + x_pos, y - y_pos, colored);
                DrawPixel(x - x_pos, y - y_pos, colored);
                DrawHorizontalLine(x + x_pos, y + y_pos, 2 * (-x_pos) + 1, colored);
                DrawHorizontalLine(x + x_pos, y - y_pos, 2 * (-x_pos) + 1, colored);
                e2 = err;
                if (e2 <= y_pos)
                {
                    err += ++y_pos * 2 + 1;
                    if (-x_pos == y_pos && e2 <= x_pos)
                    {
                        e2 = 0;
                    }
                }
                if (e2 > x_pos)
                {
                    err += ++x_pos * 2 + 1;
                }
            } while (x_pos <= 0);
        }
    }
}