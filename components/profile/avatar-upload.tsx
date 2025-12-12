'use client';

import { useState, useRef } from 'react';
import { Upload, X } from 'lucide-react';
import { Button } from '@/components/ui/button';
import api from '@/services/api';

interface AvatarUploadProps {
    currentAvatar?: string | null;
    onUploadComplete: (avatarUrl: string) => void;
}

export function AvatarUpload({ currentAvatar, onUploadComplete }: AvatarUploadProps) {
    const [preview, setPreview] = useState<string | null>(currentAvatar || null);
    const [uploading, setUploading] = useState(false);
    const fileInputRef = useRef<HTMLInputElement>(null);

    const handleFileSelect = async (e: React.ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files?.[0];
        if (!file) return;

        // Validate file
        if (file.size > 2 * 1024 * 1024) {
            alert('File must be less than 2MB');
            return;
        }

        if (!file.type.startsWith('image/')) {
            alert('File must be an image');
            return;
        }

        // Show preview
        const reader = new FileReader();
        reader.onload = (e) => setPreview(e.target?.result as string);
        reader.readAsDataURL(file);

        // Upload
        setUploading(true);
        try {
            const formData = new FormData();
            formData.append('file', file);

            const res = await api.post('/users/me/avatar', formData, {
                headers: { 'Content-Type': 'multipart/form-data' }
            });

            onUploadComplete(res.data.avatarUrl);
        } catch (error) {
            console.error('Upload failed', error);
            alert('Failed to upload avatar');
            setPreview(currentAvatar || null);
        } finally {
            setUploading(false);
        }
    };

    const handleRemove = async () => {
        try {
            await api.delete('/users/me/avatar');
            setPreview(null);
            onUploadComplete('');
        } catch (error) {
            console.error('Failed to delete avatar', error);
        }
    };

    return (
        <div className="flex items-center gap-4">
            <div className="relative">
                {preview ? (
                    <img
                        src={preview}
                        alt="Avatar"
                        className="w-24 h-24 rounded-full object-cover border-2 border-slate-200 dark:border-slate-600"
                    />
                ) : (
                    <div className="w-24 h-24 rounded-full bg-slate-200 dark:bg-slate-700 flex items-center justify-center">
                        <span className="text-3xl text-slate-400">?</span>
                    </div>
                )}
                {preview && (
                    <button
                        onClick={handleRemove}
                        className="absolute top-0 right-0 bg-red-500 text-white rounded-full p-1 hover:bg-red-600"
                    >
                        <X className="h-3 w-3" />
                    </button>
                )}
            </div>

            <div>
                <input
                    ref={fileInputRef}
                    type="file"
                    accept="image/*"
                    className="hidden"
                    onChange={handleFileSelect}
                />
                <Button
                    variant="outline"
                    onClick={() => fileInputRef.current?.click()}
                    isLoading={uploading}
                >
                    <Upload className="h-4 w-4 mr-2" />
                    {preview ? 'Change Avatar' : 'Upload Avatar'}
                </Button>
                <p className="text-xs text-slate-500 dark:text-slate-400 mt-1">
                    Max 2MB • JPG, PNG, GIF
                </p>
            </div>
        </div>
    );
}
