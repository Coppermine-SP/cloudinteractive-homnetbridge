using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HomNetBridge.Services
{
    public static class ElevatorService
    {
        public enum ElevatorDirection { Stop = 0, Down = 1, Up = 2 }
        public static readonly string[] DirectionString =
        {
            "층에 있습니다",
            "층에서 내려가고 있습니다",
            "층에서 올라가고 있습니다"
        };

        public static int FloorStringToInt(string floor)
        {
            if (Int32.TryParse(floor, out int result))
                return result;
            else
                return -Int32.Parse(floor.Replace("B", ""));
        }

        public static string FloorIntToString(int floor)
        {
            if (floor > 0)
                return floor.ToString();
            else
                return $"B{-floor}";
        }

        private const string _notifyTitle = "엘리베이터 호출";
        private const string _notifyTag = "elevator-service";
        private static object _updateLock = new object();

        private static int _currentFloor = 0;
        private static ElevatorDirection _currentDirection = 0;
        private static int _lastNotifyFloor = 0;

        private static bool _isCalled = false;
        private static bool _isFirstUpdate = true;
        private static bool _isHeadingDest = false;
        private static bool _isNearCalled = false;

        private static int _refFloor;
        private static int _notifyThreshold;

        private static string _getCurrentFloorString => FloorIntToString(_currentFloor);
        private static int _getDistance => Math.Abs(_currentFloor - _refFloor);

        public static void Init(int refFloor, int notifyThreshold)
        {
            _refFloor = refFloor;
            _notifyThreshold = notifyThreshold;
        }

        public static void ElevatorCalled()
        {
            if (_isCalled)
            {
                Logging.Print("Previous request have not been properly cleaned up. force cleanup.", Logging.LogType.Warn);
                ElevatorArrived(true);
            }

            lock (_updateLock)
            {
                Logging.Print("Elevator Called.");
                _isCalled = true;
            }
        }

        public static void ElevatorArrived(bool forceCleanup=false)
        {
            if(! _isCalled) return;

            lock (_updateLock)
            {
                _isCalled = false;
                _isFirstUpdate = true;
                _isHeadingDest = false;
                _isNearCalled = false;
                Logging.Print("Elevator arrived. reset all states.");

                if(!forceCleanup) HAService.Notify(_notifyTitle, "엘리베이터가 도착했습니다.", HAService.NotifyLevel.TimeSensitive, _notifyTag);
            }
        }

        public static void UpdateFloor(string floor, int direction)
        {
            if (!_isCalled) return;

            //ISSUE: SLB에서 floor가 0으로 반환하는 경우 존재.
            if (floor == "0") return;

            lock (_updateLock)
            {
                var floorInt = FloorStringToInt(floor);
                _currentDirection = (ElevatorDirection)direction;

                if (floorInt != _currentFloor)
                {
                    Logging.Print($"Floor update: ({floor}, direction={direction}).");
                    _currentFloor = floorInt;

                    //엘리베이터가 목적지로 향하는 방향인지 체크
                    if ((_refFloor >= _currentFloor) && direction == 2) _isHeadingDest = true;
                    else if ((_refFloor < _currentFloor) && direction == 1) _isHeadingDest = true;
                    else _isHeadingDest = false;

                    if (_isFirstUpdate)
                    {
                        Logging.Print("FirstUpdate notify push.");
                        HAService.Notify(_notifyTitle, $"엘리베이터를 호출하였습니다. 현재 {_getCurrentFloorString}{DirectionString[(int)_currentDirection]}.", HAService.NotifyLevel.TimeSensitive, _notifyTag);
                        _lastNotifyFloor = floorInt;
                        _isFirstUpdate = false;
                    }
                    else
                    {
                        if (Math.Abs(_currentFloor - _lastNotifyFloor) >= _notifyThreshold)
                        {
                            Logging.Print($"notifyThreshold reached. floor notify push.(lastNotifyFloor={_lastNotifyFloor})");
                            HAService.Notify(_notifyTitle, $"현재 {_getCurrentFloorString}{DirectionString[(int)_currentDirection]}.", HAService.NotifyLevel.TimeSensitive, _notifyTag);
                            _lastNotifyFloor = floorInt;
                        }

                        if (_isHeadingDest && _getDistance <= 3)
                        {
                            if (_isNearCalled) return;
                            Logging.Print($"Elevator expected arrival. arrival expected notify push.");

                            if (_currentDirection == ElevatorDirection.Down)
                                HAService.Notify(_notifyTitle, $"엘리베이터가 곧 도착합니다.", HAService.NotifyLevel.TimeSensitive, _notifyTag);
                            else
                                HAService.Notify(_notifyTitle, $"엘리베이터가 도착지 근처에 있습니다.", HAService.NotifyLevel.TimeSensitive, _notifyTag);
                            _isNearCalled = true;
                        }
                        else
                        {
                            _isNearCalled = false;
                        }
                    }
                }
            }
        }


    }
}
