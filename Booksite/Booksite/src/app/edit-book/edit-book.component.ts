import { Component, Inject, Input, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { SnackNotifyComponent } from '../snack-notify/snack-notify.component';
import { Location } from '@angular/common';
import { BookService } from '../services/book.service';
import { MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { createIsbnValidator, createNumValidator, createYearValidator } from '../helpers/custom-validators';
import { BookDetails } from '../models/books.model';

@Component({
  selector: 'app-edit-book',
  templateUrl: './edit-book.component.html',
  styleUrls: ['./edit-book.component.css']
})
export class EditBookComponent {
  availableGenres: string[] = ['novel'];
  coverFile?: string;
  @ViewChild('editbookForm') editbookForm: any;
  editBookForm: FormGroup;
  isbn = new FormControl('', [Validators.required, createIsbnValidator()]);
  title = new FormControl('', [Validators.required, Validators.minLength(3)]);
  author = new FormControl('', [Validators.required]);
  genre = new FormControl('', [Validators.required]);
  quantity = new FormControl('', [Validators.required, createNumValidator()]);
  price = new FormControl('', [Validators.required, createNumValidator()]);
  publishYear = new FormControl('', [Validators.required, createYearValidator()]);

  constructor(
    private formBuilder: FormBuilder,
    private authService: AuthService,
    private bookService: BookService,
    private router: Router,
    private snack: SnackNotifyComponent,
    private location: Location,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {

    this.bookService.getGenres().subscribe(
      (allGenres: string[]) => {
        this.availableGenres = allGenres;
      },
      (error) => {
        this.availableGenres = ['novel'];
      }
    );

    this.editBookForm = this.formBuilder.group({
      isbn: this.isbn,
      title: this.title,
      author: this.author,
      genre: this.genre,
      quantity: this.quantity,
      price: this.price,
      publishYear: this.publishYear
    });

    if (this.data.book) {
      const details = this.data.book as BookDetails;
      this.isbn.setValue(details.isbn);
      this.title.setValue(details.title);
      this.author.setValue(details.author);
      this.genre.setValue(details.genre);
      this.quantity.setValue(details.quantity);
      this.price.setValue(details.price);
      this.publishYear.setValue(details.publishYear);
      this.coverFile = details.coverFile;
    }
  }

  changeGenre(e: any) {
    this.genre.setValue(e.target.value, {
      onlySelf: true
    })
  }

  sendForm() {
    if (this.editBookForm.valid) {
      const editedBook: BookDetails = this.editBookForm.value;
      editedBook.coverFile = this.coverFile;
      this.bookService.editBookDetails(editedBook).subscribe(
        (serverBookDetails) => {
          this.snack.openSnackBar('Successfuly changed book details', 'Ok');
          this.forceReload();
        },
        (err) => {
          this.snack.openSnackBar('Unable to change book details', 'Ok');
          this.forceReload();
        });
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
