using System.Text;

namespace Nihlen.Gamespy;

public class GamespyReader
{
    private readonly byte[] _readBuffer;
    private int _offset;

    public bool HasData => _offset < _readBuffer.Length;

    public GamespyReader(byte[] buffer)
    {
        _readBuffer = buffer;
    }

    public byte ReadByte()
    {
        if (_offset == _readBuffer.Length)
        {
            return 0;
        }

        return _readBuffer[_offset++];
    }

    public byte PeekByte()
    {
        if (_offset == _readBuffer.Length)
        {
            return 0;
        }

        return _readBuffer[_offset];
    }

    public string ReadNextParam()
    {
        var sb = new StringBuilder();
        for (; _offset < _readBuffer.Length; _offset++)
        {
            if (_readBuffer[_offset] == 0)
            {
                _offset++;
                break;
            }

            sb.Append((char)_readBuffer[_offset]);
        }

        return sb.ToString();
    }
}
