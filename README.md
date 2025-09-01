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
Unity 에디터 확장
빌드에 영향을 주지 않으면서 에디터에서 작업할 때 유용할 가능성이 있는 확장 도구입니다.

주요 기능

하이어라키(Hierarchy) 개선
- 구분선: 하이어라키에 시각적인 구분선 추가
- 계층 구조 색 구분: 계층 구조의 깊이에 따라 개별 색상으로 구분되고, 각 오브젝트간에도 알파 차이로 구분을 쉽도록 함
- 계층 구조 라인: 계층 구조를 시각적으로 표시하는 라인
- 배경색: 계층 구조와 상관없이 특정 오브젝트에 배경색을 설정
- 컴포넌트 아이콘: 게임오브젝트에 포함된 컴포넌트를 아이콘으로 표시
- 노트 기능: 게임오브젝트에 메모가 존재하면 툴팁으로 표시

인스펙터(Inspector) 확장
- 노트 편집: 게임오브젝트에 메모 추가 및 편집
- 배경색 설정: 게임오브젝트의 배경색 설정

씬 뷰(Scene View) Gizmo
- 노트 표시: 씬 뷰에서 게임오브젝트의 메모 표시
- 외곽선 표시: 선택한 오브젝트의 외곽선을 배경색으로 표시

특징
- 빌드에 영향을 주지 않는 에디터 전용 기능
- 외부 저장의 예시로 RocksDB를 사용했지만 LevelDB, Redis, SQLite 등 다른 DB로도 쉽게 변경 가능

요구사항
- Unity 6에서 작성되었으며, 이하 버전에서는 테스트되지 않았습니다.