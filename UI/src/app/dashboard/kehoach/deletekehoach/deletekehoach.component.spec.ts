import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DeletekehoachComponent } from './deletekehoach.component';

describe('DeletekehoachComponent', () => {
  let component: DeletekehoachComponent;
  let fixture: ComponentFixture<DeletekehoachComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DeletekehoachComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DeletekehoachComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
