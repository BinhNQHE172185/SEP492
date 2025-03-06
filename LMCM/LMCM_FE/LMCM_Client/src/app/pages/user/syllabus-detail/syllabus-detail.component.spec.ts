import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SyllabusDetailComponent } from './syllabus-detail.component';

describe('SyllabusDetailComponent', () => {
  let component: SyllabusDetailComponent;
  let fixture: ComponentFixture<SyllabusDetailComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SyllabusDetailComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SyllabusDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
