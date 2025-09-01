# UnityEditorExtension
## KingGodGeneralEditorExtensionPro
A Unity editor extension tool that enhances your workflow in the editor. Never affects the build, but might be useful for working in the editor.

Main Features

Hierarchy View Enhancements
- Separators: Add visual separators in the hierarchy
- Depth Color: Distinguish hierarchy depth with different colors and alpha variations between objects
- Structure Lines: Visualize parent-child relationships with customizable lines
- Background Color: Set custom background colors for specific objects 
regardless of hierarchy
- Component Icons: Display component icons for quick identification
- Note Display: Show object notes as tooltips when hovering over items

Inspector Extensions
- Note Editing: Add and edit notes for game objects
- Background Color Settings: Configure custom background colors for hierarchy items

Scene View Gizmos
- Note Display: Show object notes directly in the Scene view
- Outline Display: Highlight selected objects with outline using their background colors
Features
- Editor-only functionality with no impact on builds
- RocksDB is used as an example for external storage, but it can be easily changed to other databases such as LevelDB, Redis, SQLite, etc.

Requirements
- Developed in Unity 6, not tested in lower versions
---
Unity ������ Ȯ��
���忡 ������ ���� �����鼭 �����Ϳ��� �۾��� �� ������ ���ɼ��� �ִ� Ȯ�� �����Դϴ�.

�ֿ� ���

���̾��Ű(Hierarchy) ����
- ���м�: ���̾��Ű�� �ð����� ���м� �߰�
- ���� ���� �� ����: ���� ������ ���̿� ���� ���� �������� ���еǰ�, �� ������Ʈ������ ���� ���̷� ������ ������ ��
- ���� ���� ����: ���� ������ �ð������� ǥ���ϴ� ����
- ����: ���� ������ ������� Ư�� ������Ʈ�� ������ ����
- ������Ʈ ������: ���ӿ�����Ʈ�� ���Ե� ������Ʈ�� ���������� ǥ��
- ��Ʈ ���: ���ӿ�����Ʈ�� �޸� �����ϸ� �������� ǥ��

�ν�����(Inspector) Ȯ��
- ��Ʈ ����: ���ӿ�����Ʈ�� �޸� �߰� �� ����
- ���� ����: ���ӿ�����Ʈ�� ���� ����

�� ��(Scene View) Gizmo
- ��Ʈ ǥ��: �� �信�� ���ӿ�����Ʈ�� �޸� ǥ��
- �ܰ��� ǥ��: ������ ������Ʈ�� �ܰ����� �������� ǥ��

Ư¡
- ���忡 ������ ���� �ʴ� ������ ���� ���
- �ܺ� ������ ���÷� RocksDB�� ��������� LevelDB, Redis, SQLite �� �ٸ� DB�ε� ���� ���� ����

�䱸����
- Unity 6���� �ۼ��Ǿ�����, ���� ���������� �׽�Ʈ���� �ʾҽ��ϴ�.