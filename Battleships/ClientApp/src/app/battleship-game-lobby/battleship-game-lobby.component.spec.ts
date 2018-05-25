import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BattleshipGameLobbyComponent } from './battleship-game-lobby.component';

describe('BattleshipGameLobbyComponent', () => {
  let component: BattleshipGameLobbyComponent;
  let fixture: ComponentFixture<BattleshipGameLobbyComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [BattleshipGameLobbyComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BattleshipGameLobbyComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
