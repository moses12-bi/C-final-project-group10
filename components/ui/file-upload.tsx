'use client';

import { useState, useRef, DragEvent } from 'react';
import { Upload, X, File } from 'lucide-react';
import { Button } from './button';

export interface FileUploadProps {
    onFileSelect: (file: File) => void;
    maxSize?: number; // in MB
    acceptedTypes?: string[];
    uploading?: boolean;
}

export function FileUpload({ onFileSelect, maxSize = 10, acceptedTypes, uploading = false }: FileUploadProps) {
    const [isDragging, setIsDragging] = useState(false);
    const [selectedFile, setSelectedFile] = useState<File | null>(null);
    const fileInputRef = useRef<HTMLInputElement>(null);

    const handleDrag = (e: DragEvent<HTMLDivElement>) => {
        e.preventDefault();
        e.stopPropagation();
    };

    const handleDragIn = (e: DragEvent<HTMLDivElement>) => {
        e.preventDefault();
        e.stopPropagation();
        setIsDragging(true);
    };

    const handleDragOut = (e: DragEvent<HTMLDivElement>) => {
        e.preventDefault();
        e.stopPropagation();
        setIsDragging(false);
    };

    const handleDrop = (e: DragEvent<HTMLDivElement>) => {
        e.preventDefault();
        e.stopPropagation();
        setIsDragging(false);

        const files = e.dataTransfer.files;
        if (files && files.length > 0) {
            handleFile(files[0]);
        }
    };

    const handleFileInput = (e: React.ChangeEvent<HTMLInputElement>) => {
        const files = e.target.files;
        if (files && files.length > 0) {
            handleFile(files[0]);
        }
    };

    const handleFile = (file: File) => {
        // Check file size
        if (file.size > maxSize * 1024 * 1024) {
            alert(`File size must be less than ${maxSize}MB`);
            return;
        }

        // Check file type if provided
        if (acceptedTypes && acceptedTypes.length > 0) {
            const fileExtension = '.' + file.name.split('.').pop()?.toLowerCase();
            if (!acceptedTypes.includes(fileExtension)) {
                alert(`File type not accepted. Accepted types: ${acceptedTypes.join(', ')}`);
                return;
            }
        }

        setSelectedFile(file);
    };

    const handleUpload = () => {
        if (selectedFile) {
            onFileSelect(selectedFile);
            setSelectedFile(null);
            if (fileInputRef.current) {
                fileInputRef.current.value = '';
            }
        }
    };

    const handleClear = () => {
        setSelectedFile(null);
        if (fileInputRef.current) {
            fileInputRef.current.value = '';
        }
    };

    return (
        <div className="w-full">
            <div
                className={`relative border-2 border-dashed rounded-lg p-8 text-center transition-colors ${isDragging
                        ? 'border-blue-500 bg-blue-50'
                        : 'border-slate-300 bg-slate-50 hover:border-slate-400'
                    }`}
                onDragEnter={handleDragIn}
                onDragLeave={handleDragOut}
                onDragOver={handleDrag}
                onDrop={handleDrop}
            >
                <input
                    ref={fileInputRef}
                    type="file"
                    className="hidden"
                    onChange={handleFileInput}
                    accept={acceptedTypes?.join(',')}
                />

                {selectedFile ? (
                    <div className="space-y-4">
                        <div className="flex items-center justify-center gap-3 p-4 bg-white rounded-lg border border-slate-200">
                            <File className="h-6 w-6 text-slate-600" />
                            <div className="flex-1 text-left">
                                <div className="font-medium text-slate-900">{selectedFile.name}</div>
                                <div className="text-sm text-slate-500">
                                    {(selectedFile.size / 1024 / 1024).toFixed(2)} MB
                                </div>
                            </div>
                            <button
                                onClick={handleClear}
                                className="text-slate-400 hover:text-red-600"
                                disabled={uploading}
                            >
                                <X className="h-5 w-5" />
                            </button>
                        </div>
                        <Button onClick={handleUpload} isLoading={uploading} className="w-full">
                            Upload File
                        </Button>
                    </div>
                ) : (
                    <div className="space-y-4">
                        <Upload className="h-12 w-12 text-slate-400 mx-auto" />
                        <div>
                            <p className="text-slate-700 font-medium mb-1">
                                Drop your file here, or{' '}
                                <button
                                    onClick={() => fileInputRef.current?.click()}
                                    className="text-blue-600 hover:text-blue-700 underline"
                                >
                                    browse
                                </button>
                            </p>
                            <p className="text-sm text-slate-500">
                                Maximum file size: {maxSize}MB
                                {acceptedTypes && ` • Accepted: ${acceptedTypes.join(', ')}`}
                            </p>
                        </div>
                    </div>
                )}
            </div>
        </div>
    );
}
