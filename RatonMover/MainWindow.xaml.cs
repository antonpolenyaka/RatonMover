using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;

namespace MouseMoverApp
{
    public partial class MainWindow : Window
    {
        private readonly System.Timers.Timer inactivityTimer;
        private readonly Random random = new();
        private bool isMoving = false;
        private bool isStopped = true;  // Inicialmente, la aplicación está detenida.
        private DateTime lastMouseMove;

        public MainWindow()
        {
            InitializeComponent();
            inactivityTimer = new System.Timers.Timer(1000); // Check inactivity every second
            inactivityTimer.Elapsed += CheckInactivity;
            lastMouseMove = DateTime.Now;
            MouseMove += OnMouseMove;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            lastMouseMove = DateTime.Now;
            if (isMoving)
            {
                isMoving = false;
            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            isStopped = false;
            inactivityTimer.Start();
            UpdateButtonStates();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            isStopped = true;
            inactivityTimer.Stop();
            UpdateButtonStates();
        }

        private void CheckInactivity(object? sender, System.Timers.ElapsedEventArgs _)
        {
            if (isStopped) return;

            TimeSpan inactivityDuration = DateTime.Now - lastMouseMove;
            if (inactivityDuration.TotalSeconds >= 60 && !isMoving)
            {
                isMoving = true;
                MoveMouseRandomly();
            }
        }

        private async void MoveMouseRandomly()
        {
            while (isMoving && !isStopped)
            {
                int xMove = random.Next(-50, 51); // Movimiento aleatorio en el eje X (positivo o negativo)
                int yMove = random.Next(-50, 51); // Movimiento aleatorio en el eje Y (positivo o negativo)

                Console.WriteLine($"Moving cursor by x: {xMove}, y: {yMove}"); // Verificar el movimiento en la consola
                MoveCursor(xMove, yMove);

                int delay = random.Next(1000, 5001); // Delay entre 1 y 5 segundos
                await Task.Delay(delay);
            }
        }

        private static void MoveCursor(int xOffset, int yOffset)
        {
            Point currentMousePosition = GetMousePosition();

            // Obtener el tamaño de la pantalla
            int screenWidth = (int)SystemParameters.PrimaryScreenWidth;
            int screenHeight = (int)SystemParameters.PrimaryScreenHeight;

            // Calcular la nueva posición asegurándonos de que no salga de los bordes
            int newX = Math.Max(0, Math.Min(screenWidth - 1, (int)currentMousePosition.X + xOffset));
            int newY = Math.Max(0, Math.Min(screenHeight - 1, (int)currentMousePosition.Y + yOffset));

            Console.WriteLine($"Moving cursor to x: {newX}, y: {newY}"); // Verificar nueva posición
            bool success = SetCursorPos(newX, newY);

            if (!success)
            {
                Console.WriteLine("Error moving cursor");
            }
        }

        [LibraryImport("user32.dll", EntryPoint = "SetCursorPos")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool SetCursorPos(int X, int Y);

        [LibraryImport("user32.dll", EntryPoint = "GetCursorPos")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool GetCursorPos(out POINT lpPoint);

        private static Point GetMousePosition()
        {
            GetCursorPos(out POINT point);
            return new Point(point.X, point.Y);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int X;
            public int Y;
        }

        private void UpdateButtonStates()
        {
            StartButton.IsEnabled = isStopped; // El botón de inicio está habilitado si está detenido
            StopButton.IsEnabled = !isStopped; // El botón de detener está habilitado si está en marcha
        }
    }
}
