namespace SCADAcore
{
    public enum UserRole { USER, ADMIN }

    // RealTimeDriver, SimulationDriver
    public enum Driver { RTD, SD }

    //                              1      2     3
    public enum AlarmPriority { LOW = 1, MEDIUM, HIGH }

    public enum AlarmType { LOW, HIGH, NOT_ACTIVATED }

    public enum TagType { AI, AO, DI, DO }
}