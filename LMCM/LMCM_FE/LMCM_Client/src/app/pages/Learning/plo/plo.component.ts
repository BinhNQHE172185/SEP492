import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { CardModule } from 'primeng/card';

@Component({
  selector: 'app-plo-mapping',
  standalone: true,
  imports: [CommonModule, TableModule, CardModule],
  templateUrl:   './plo.component.html'
    
  
  
})
export class PloComponent {
  programLearningOutcomes = [
    { no: 1, name: 'Critical Thinking', description: 'Apply critical thinking and problem-solving skills' },
    { no: 2, name: 'Technical Knowledge', description: 'Demonstrate comprehensive understanding of computing principles' },
    { no: 3, name: 'Professional Ethics', description: 'Practice professional ethics in computing' }
  ];

  ploMapping = [
    { subject: 'IT4409', plo1: true, plo2: false, plo3: true },
    { subject: 'IT3103', plo1: true, plo2: true, plo3: false },
    { subject: 'IT2030', plo1: false, plo2: true, plo3: true }
  ];
}
