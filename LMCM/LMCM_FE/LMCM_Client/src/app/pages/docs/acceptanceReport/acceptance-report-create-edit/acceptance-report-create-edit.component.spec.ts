import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AcceptanceReportCreateEditComponent } from './acceptance-report-create-edit.component';

describe('AcceptanceReportCreateEditComponent', () => {
  let component: AcceptanceReportCreateEditComponent;
  let fixture: ComponentFixture<AcceptanceReportCreateEditComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AcceptanceReportCreateEditComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AcceptanceReportCreateEditComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
