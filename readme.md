Merona.JsonProtocol.cs
====

Merona에 연속된 Json으로 이루어진 프로토콜을 지원합니다.<br>
이 확장을 사용하여 telnet을 이용한 서버 테스트를 용이하게 진행할 수 있습니다.

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
연속된 두 json 패킷 사이에는 아무런 구분자도 붙이지 않습니다.(파서가 인식할 수 있는 줄바꿈, 띄어쓰기 등의 문자는 허용됩니다.)<br>
루트 오브젝트가 열리고 닫히는 것으로 패킷과 패킷 사이를 구분합니다.