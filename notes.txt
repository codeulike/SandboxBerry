
VS bug leads to problems with Partner WSDL v 32
fix here:
https://developer.salesforce.com/forums/ForumsMain?id=906F0000000Ai2JIAS
basically:
In Reference.cs
search and replace
ListViewRecordColumn[][]
and replace it with
ListViewRecordColumn[]

To generate Schema:
xsd SandboxberryLib.dll /type:SbbInstructionSet




icon from icons8
http://icons8.com/license/
