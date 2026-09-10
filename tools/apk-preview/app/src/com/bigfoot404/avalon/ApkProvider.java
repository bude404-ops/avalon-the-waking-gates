package com.bigfoot404.avalon;

import android.content.ContentProvider;
import android.content.ContentValues;
import android.database.Cursor;
import android.database.MatrixCursor;
import android.net.Uri;
import android.os.ParcelFileDescriptor;
import java.io.File;
import java.io.FileNotFoundException;

/** Serves the downloaded update APK to the system package installer. */
public class ApkProvider extends ContentProvider {
    public static final String AUTHORITY = "com.bigfoot404.avalon.apk";

    @Override public boolean onCreate() { return true; }

    private File apkFile() {
        return new File(getContext().getFilesDir(), "avalon-update.apk");
    }

    @Override
    public ParcelFileDescriptor openFile(Uri uri, String mode) throws FileNotFoundException {
        return ParcelFileDescriptor.open(apkFile(), ParcelFileDescriptor.MODE_READ_ONLY);
    }

    @Override
    public Cursor query(Uri uri, String[] projection, String selection,
                        String[] args, String sortOrder) {
        // Package installer asks for _display_name / _size
        if (projection == null) projection = new String[]{"_display_name", "_size"};
        MatrixCursor c = new MatrixCursor(projection);
        Object[] row = new Object[projection.length];
        for (int i = 0; i < projection.length; i++) {
            switch (projection[i]) {
                case "_display_name": row[i] = "AVALON-TEST-UPDATE.apk"; break;
                case "_size":         row[i] = apkFile().length();        break;
                default:              row[i] = null;                       break;
            }
        }
        c.addRow(row);
        return c;
    }

    @Override public String getType(Uri uri) { return "application/vnd.android.package-archive"; }
    @Override public Uri insert(Uri uri, ContentValues v) { return null; }
    @Override public int delete(Uri uri, String s, String[] a) { return 0; }
    @Override public int update(Uri uri, ContentValues v, String s, String[] a) { return 0; }
}
