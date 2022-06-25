import { HttpEventType, HttpResponse } from '@angular/common/http';
import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { BookDetails } from '../models/books.model';
import { BookCoverService } from '../services/book-cover.service';
import { SnackNotifyComponent } from '../snack-notify/snack-notify.component';

@Component({
  selector: 'app-book-cover-upload',
  templateUrl: './book-cover-upload.component.html',
  styleUrls: ['./book-cover-upload.component.css']
})
export class BookCoverUploadComponent {
  selectedFile?: FileList;
  isbn = '';
  selectedFileName: string = '';

  progressInfo: any;
  message: string = '';
  preview: string = '';

  constructor(
    private uploadService: BookCoverService,
    private router: Router,
    private snack: SnackNotifyComponent,
    public dialogRef: MatDialogRef<BookCoverUploadComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    const book = this.data.book as BookDetails;
    if (book) {
      this.isbn = this.data.book.isbn;
    } else {
      console.log('fail');
    }
  }

  selectFile(event: any): void {
    this.message = '';
    this.progressInfo = '';
    this.selectedFileName = '';
    this.selectedFile = event.target.files;
    if (this.selectedFile) {
      const numberOfFiles = this.selectedFile.length;
      if (numberOfFiles != 0) {
        for (let i = 0; i < 1; i++) {
          const reader = new FileReader();
          reader.onload = (e: any) => {
            console.log(e.target.result);
            this.preview = e.target.result;
          };
          reader.readAsDataURL(this.selectedFile[i]);
          this.selectedFileName = this.selectedFile[i].name;
        }
      }
    }
  }

  upload(idx: number, file: File): void {
    this.progressInfo = { value: 0, fileName: file.name };
    if (file) {
      this.uploadService.uploadCover(file, this.isbn).subscribe(
        (event: any) => {
          if (event.type === HttpEventType.UploadProgress) {
            this.progressInfo.value = Math.round(
              (100 * event.loaded) / event.total
            );
          } else if (event instanceof HttpResponse) {

            const msg = 'Cover uploaded successfully';
            this.dialogRef.close();
            this.snack.openSnackBar(msg, 'ok');
            this.forceReload();
          }
        },
        (err: any) => {
          this.progressInfo.value = 0;
          const msg = 'Error occured while uploading cover';
          this.dialogRef.close();
          this.snack.openSnackBar(msg, 'ok');
          this.forceReload();
        }
      );
    }
  }

  uploadFile(): void {
    this.message = '';
    if (this.selectedFile) {
      this.upload(0, this.selectedFile[0]);
    }
  }

  forceReload() {
    let currentUrl = this.router.url;
    this.router.navigateByUrl('/', { skipLocationChange: true }).then(() => {
      this.router.navigate([currentUrl]);
      console.log(currentUrl);
    });
  }
}
