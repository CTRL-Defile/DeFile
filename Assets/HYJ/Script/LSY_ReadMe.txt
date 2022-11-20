# 이상윤 - 전투파트 ReadMe

1. 전투화면 기초 UI
1.1 Bear, Orc를 Shop_UnitList에 랜덤 생성
1.2 Shop_UnitList 클릭 시 대기열에 Unit 객체 생성
1.3 Shop_Refresh, Exp, Coin 등 기능 구현

2. Map
2.1 


고찰 - 유닛 위치는 어디서 정해주는게 맞는가?
1. Tile
2. Drag

순서 : Tile.TriggerEXIT -> Drag.MouseButtonDOWN -> Drag.MouseButtonUP -> Tile.TriggerEnter

최종적으로 실행되는 메소드가 Tile 트리거라, Tile에서 유닛의 위치를 고쳐주는 것으로 구현했는데 생각해보니
과연 타일에서 유닛 위치를 조정시키는 것이 맞나, 에 대한 의문이 생김. 왜냐, 이는 MouseDOWN 시 유닛의 y를 증가시켜 collider에 닿지 않는 상태일 때는
잘 동작하나, 드래그 중에도 collider에 닿고 있다면, 최종 순서가 TriggerEnter가 아니므로 Tile이 최종적인 유닛의 위치를 고정시킬 수 없게됨.

근데 흐름상 Drag부분에선 고칠 수 없음. 그렇다고 이거 고치자고 드래그 중에도 콜라이더에 닿도록 타일 콜라이더를 크게 만든다?
굳이? 라는 느낌이 강함..

그럼 Character에서 위치를 정해줄까? -> 현재는 전투없이 배치만 생각하고 있으니 그렇게 해도 될 것 같으나,, 다른 전투 파트에서 어떻게 쓸 지 모르므로,,,
이것도 쉽게 접근하기 힘들다고 봄.




