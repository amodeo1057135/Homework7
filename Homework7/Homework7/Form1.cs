using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace Homework7
{
    public partial class Form1 : Form
    {
        public int trials = 100;
        public int paths = 100;
        public int lambdaValue = 5;

        public Bitmap bmap;
        public Graphics grap;
        public Random rand;

        private Pen histoPen = new Pen(Color.OrangeRed, 25);


        public Form1()
        {
            InitializeComponent();
            rand = new Random();
            textBox1.Text = lambdaValue.ToString();
            textBox2.Text = trials.ToString();
            textBox3.Text = paths.ToString();

            bmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            grap = Graphics.FromImage(bmap);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            grap.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            grap.Clear(Color.White);
            pictureBox1.Image = bmap;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            grap.Clear(Color.White);
            var minValue = new Point(0, 0);
            var maxValue = new Point(trials, paths);

            var mypoints = new List<PointF>();
            var dictionaryXValues = new Dictionary<int, int>();

            var successProb = lambdaValue/ (double)paths;

            var plotWidth = Convert.ToInt32(pictureBox1.Width * 0.7f);
            Rectangle plotWindow = new Rectangle(10, 10, plotWidth - 20, pictureBox1.Height - 20);
            Rectangle histogramWindow = new Rectangle(plotWindow.X + plotWindow.Width + 10, 10, pictureBox1.Width - plotWindow.Width - 20, pictureBox1.Height - 20);

            for (int t = 0; t < trials; t++)
            {
                var yValue = 0;
                for (int x = 0; x < paths; x++)
                {
                    var uniform = rand.NextDouble();
                    if (uniform < successProb)
                    {
                        yValue++;
                        mypoints.Add(new Point(virtualX(x, 0, trials, plotWidth),
                            virtualY(yValue, 0, trials, pictureBox1.Height)));
                    }
                }
                if (dictionaryXValues.ContainsKey(yValue))
                {
                    dictionaryXValues[yValue] += 1;
                } else
                {
                    dictionaryXValues.Add(yValue, 1);
                }
            }

            var valuesArray = mypoints.ToArray();
            grap.DrawLine(Pens.BlueViolet, new Point(0, pictureBox1.Height), valuesArray[0]);
            for (int i = 0; i < valuesArray.Length - 1; i++)
            {
                grap.DrawLine(Pens.BlueViolet, valuesArray[i], valuesArray[i + 1]);
            }

            List<PointF> histogram = new List<PointF>();

            foreach(KeyValuePair<int, int> value in dictionaryXValues)
            {
                var pointF = fromRealToVirtual(new PointF(0, value.Key), minValue, maxValue, histogramWindow);
                //pointF.Y = pointF.Y + 25;
                var pointH = fromRealToVirtual(new PointF(value.Value, value.Key), minValue, maxValue, histogramWindow);
                //pointF.Y = pointF.Y + 25;
                histogram.Add(pointF);
                histogram.Add(pointH);
            }

            var histoPoints = histogram.ToArray();

            for (int i = 0; i < histoPoints.Length-1; i++)
            {
                grap.DrawLine(histoPen, histoPoints[i], histoPoints[i + 1]);
                i++;
            }
            pictureBox1.Image = bmap;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                lambdaValue = Int32.Parse(textBox1.Text);
            }
        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.Text != "")
            {
                trials = Int32.Parse(textBox2.Text);
            }
        }
        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                paths = Int32.Parse(textBox3.Text);
            }
        }

        private int virtualX(int x, int minX, int maxX, double W)
        {
            return (int)(W * (double)(x - minX) / (maxX - minX));
        }

        private int virtualY(int y, int minY, int maxY, double H)
        {
            return (int)(H - H * (double)(y - minY) / (maxY - minY));
        }

        private PointF fromRealToVirtual(PointF point, Point minValue, Point maxValue, Rectangle rect)
        {
            float newX = maxValue.X - minValue.X == 0 ? 0 : (rect.Left + rect.Width * (point.X - minValue.X) / (maxValue.X - minValue.X));
            float newY = maxValue.Y - minValue.Y == 0 ? 0 : (rect.Top + rect.Height - rect.Height * (point.Y - minValue.Y) / (maxValue.Y - minValue.Y));
            return new PointF(newX, newY);
        }
    }
}