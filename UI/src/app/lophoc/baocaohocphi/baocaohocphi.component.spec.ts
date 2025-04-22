import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BaocaohocphiComponent } from './baocaohocphi.component';

describe('BaocaohocphiComponent', () => {
  let component: BaocaohocphiComponent;
  let fixture: ComponentFixture<BaocaohocphiComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ BaocaohocphiComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(BaocaohocphiComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
