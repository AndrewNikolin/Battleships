import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BattleshipGameSessionComponent } from './battleship-game-session.component';

describe('BattleshipGameSessionComponent', () => {
  let component: BattleshipGameSessionComponent;
  let fixture: ComponentFixture<BattleshipGameSessionComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BattleshipGameSessionComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BattleshipGameSessionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
