# Tcp Raw Data Packet Reader
#### I am not responsible for what people do with this tool.

## About
Using this tool you are able to read the raw data from captured TCP packets from Wireshark or similar tools.

## Usage

### Requeriments
- Visual Studio 2019

### How to use
For each packet the tool fixes/reads, the method Read will be ran and that's where the magic begins. There you can call the BufferReader variable from the method and read from the current packet that is being read.

Example:
> byte ProtocolID = br.ReadByte();

> ushort msgType = br.ReadUInt16();

## Credits
- bubisnew (for the help with figuring out how to fix a problem with more than one packet in one line and for the ReadPackets() method).
