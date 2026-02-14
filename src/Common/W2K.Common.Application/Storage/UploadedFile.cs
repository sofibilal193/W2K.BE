namespace DFI.Common.Application.Storage;

public record UploadedFile
{
    public string Path { get; init; } = "";

    public string? Name { get; init; }

    public byte[]? Data { get; private set; }

    public int? TtlDays { get; init; }

    public IDictionary<string, string>? MetaData { get; private set; }

    #region Constructors

    public UploadedFile(
        string path,
        string? name = null,
        byte[]? data = null,
        int? ttlDays = null,
        IDictionary<string, string>? metaData = null)
    {
        Path = path;
        Name = name;
        Data = data;
        TtlDays = ttlDays;
        MetaData = metaData ?? new Dictionary<string, string>();
    }

    public UploadedFile(string name, byte[] data)
    {
        Name = name;
        Data = data;
    }

    public UploadedFile(
        string path,
        string name,
        IDictionary<string, string> metaData)
    {
        Path = path;
        Name = name;
        MetaData = metaData;
    }

    #endregion

    #region Public Methods

    public void SetData(byte[] data)
    {
        Data = data;
    }

    public void SetMetaData(IDictionary<string, string>? metaData)
    {
        MetaData = metaData;
    }

    #endregion
}
