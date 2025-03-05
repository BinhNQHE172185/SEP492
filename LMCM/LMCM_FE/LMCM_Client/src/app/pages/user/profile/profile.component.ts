import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms'; // ✅ Import FormsModule
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
    FormsModule, // ✅ Thêm FormsModule vào đây
    CardModule,
    ButtonModule,
    DialogModule,
    InputTextModule,
    FileUploadModule
  ],
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent {
  userDialog: boolean = false;
  submitted: boolean = false;
  uploadedImage: string | null = null;

  user = {
    name: 'Nguyễn Văn A',
    email: 'nguyenvana@example.com',
    avatar: ''
  };

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
    this.userDialog = false;
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
