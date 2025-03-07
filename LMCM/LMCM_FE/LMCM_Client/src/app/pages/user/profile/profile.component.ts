import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { CardModule } from 'primeng/card';

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

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.getUserProfile();
  }

  getUserProfile() {
    const userId = localStorage.getItem('userId'); 
    if (!userId) {
      console.error('Không tìm thấy userId, vui lòng đăng nhập lại.');
      return;
    }

    this.http.post<any>('http://localhost:5035/api/User/profile', 
        JSON.stringify(userId), {
          headers: { 'Content-Type': 'application/json' }
        }).subscribe(response => {
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
