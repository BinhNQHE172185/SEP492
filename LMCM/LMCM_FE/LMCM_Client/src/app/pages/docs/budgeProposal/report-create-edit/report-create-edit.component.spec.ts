import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReportCreateEditComponent } from './report-create-edit.component';

describe('ReportCreateEditComponent', () => {
  let component: ReportCreateEditComponent;
  let fixture: ComponentFixture<ReportCreateEditComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ReportCreateEditComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ReportCreateEditComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
