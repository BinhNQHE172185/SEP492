import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { CardModule } from 'primeng/card';
import { UserApiService } from '../../../apis/userAPIs/user-api.service';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, CardModule],
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {
  user = {
    name: '',
    email: '',
    avatar: ''
  };

  constructor(private http: HttpClient, private userApiService: UserApiService) {}

  ngOnInit() {
    this.getUserProfile();
  }

  getUserProfile() {
    this.userApiService.getProfile().subscribe(response => {
          if (response) {
            this.user = {
              name: response.name,
              email: response.email,
              avatar: response.picture
            };
          }
        }, error => {
          console.error('Lỗi khi lấy thông tin user:', error);
        });
  }
}
