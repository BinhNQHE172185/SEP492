import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { FileUploadModule } from 'primeng/fileupload';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    CardModule,
    ButtonModule,
    DialogModule,
    InputTextModule,
    FileUploadModule
  ],
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {
  user = {
    name: '',
    email: '',
    avatar: ''
  };

  userDialog: boolean = false;
  submitted: boolean = false;
  uploadedImage: string | null = null;

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.getUserProfile();
  }

  getUserProfile() {
    const staffId = "8168EAA1-08CF-4163-8713-0A2B73B83BB4";
  
    this.http.post<any>('http://localhost:5035/api/User/profile', 
        JSON.stringify(staffId), {
          headers: { 'Content-Type': 'application/json' }
        }).subscribe(response => {
          if (response) {
            this.user.name = response.name;
            this.user.email = response.email;
            this.user.avatar = response.picture; // hoặc response.avatar nếu API trả về avatar
          }
        }, error => {
          console.error('Lỗi khi lấy thông tin user:', error);
        });
  }
  
  

  openEdit() {
    this.userDialog = true;
  }

  hideDialog() {
    this.userDialog = false;
    this.submitted = false;
  }

  saveUser() {
    if (!this.user.email) {
      this.submitted = true;
      return;
    }
    const staffId = "8168EAA1-08CF-4163-8713-0A2B73B83BB4";
    this.http.post<any>('http://localhost:5035/api/User/update', this.user)
      .subscribe(response => {
        console.log('Cập nhật thành công:', response);
        this.userDialog = false;
      }, error => {
        console.error('Lỗi khi cập nhật user:', error);
      });
  }

  onFileSelected(event: any) {
    const file = event.target.files[0];
    if (file) {
      const reader = new FileReader();
      reader.onload = (e: any) => {
        this.uploadedImage = e.target.result;
        this.user.avatar = e.target.result;
      };
      reader.readAsDataURL(file);
    }
  }
}
