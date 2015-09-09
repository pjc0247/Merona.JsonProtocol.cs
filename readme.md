Merona.JsonProtocol.cs
====

Merona�� ���ӵ� Json���� �̷���� ���������� �����մϴ�.<br>
�� Ȯ���� ����Ͽ� telnet�� �̿��� ���� �׽�Ʈ�� �����ϰ� ������ �� �ֽ��ϴ�.

```c#
[PacketId(1)]
class FooPacket : Packet {
  public String msg;
  public int num;
}
[PacketId(2)]
class BarPacket : Packet {
  public List<int> ary;
}
```
```json
{
  "id" : 1,
  "foo" : "some string",
  "num" : 1234
}
{
  "id" : 2,
  "ary" : [1,2,3,4]
}
```
���ӵ� �� json ��Ŷ ���̿��� �ƹ��� �����ڵ� ������ �ʽ��ϴ�.(�ļ��� �ν��� �� �ִ� �ٹٲ�, ���� ���� ���ڴ� ���˴ϴ�.)<br>
��Ʈ ������Ʈ�� ������ ������ ������ ��Ŷ�� ��Ŷ ���̸� �����մϴ�.