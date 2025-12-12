'use client';

import { Download } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { exportProjectPDF, exportProjectExcel, exportTasksExcel, downloadFile } from '@/services/export';
import { useState } from 'react';

interface ExportButtonsProps {
    projectId?: number;
    type: 'project' | 'tasks';
}

export function ExportButtons({ projectId, type }: ExportButtonsProps) {
    const [loading, setLoading] = useState(false);

    const handleExportPDF = async () => {
        if (!projectId) return;
        setLoading(true);
        try {
            const blob = await exportProjectPDF(projectId);
            downloadFile(blob, `project-${projectId}-report.csv`);
        } catch (error) {
            console.error('Export failed', error);
            alert('Export failed');
        } finally {
            setLoading(false);
        }
    };

    const handleExportExcel = async () => {
        setLoading(true);
        try {
            let blob;
            if (type === 'project' && projectId) {
                blob = await exportProjectExcel(projectId);
                downloadFile(blob, `project-${projectId}-export.csv`);
            } else {
                blob = await exportTasksExcel(projectId);
                downloadFile(blob, 'tasks-export.csv');
            }
        } catch (error) {
            console.error('Export failed', error);
            alert('Export failed');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="flex gap-2">
            {type === 'project' && (
                <Button
                    variant="outline"
                    size="sm"
                    onClick={handleExportPDF}
                    isLoading={loading}
                >
                    <Download className="h-4 w-4 mr-2" />
                    Export CSV
                </Button>
            )}
            <Button
                variant="outline"
                size="sm"
                onClick={handleExportExcel}
                isLoading={loading}
            >
                <Download className="h-4 w-4 mr-2" />
                Export Excel
            </Button>
        </div>
    );
}
