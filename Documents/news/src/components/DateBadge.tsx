import React from 'react';
import { useTranslation } from 'react-i18next';

interface DateBadgeProps {
  dateString: string;
  className?: string;
}

export default function DateBadge({ dateString, className = '' }: DateBadgeProps) {
  const { t, i18n } = useTranslation();

  const getTimeInfo = (dateString: string) => {
    const date = new Date(dateString);
    const now = new Date();
    const diffInMs = now.getTime() - date.getTime();
    const diffInMinutes = Math.floor(diffInMs / (1000 * 60));
    const diffInHours = Math.floor(diffInMs / (1000 * 60 * 60));
    const diffInDays = Math.floor(diffInMs / (1000 * 60 * 60 * 24));

    // Determine badge type and styling
    let type: 'live' | 'today' | 'recent' | 'older';
    let text: string;
    let bgColor: string;
    let textColor: string;
    let icon: string;

    if (diffInMinutes < 5) {
      type = 'live';
      text = 'LIVE';
      bgColor = 'bg-red-100';
      textColor = 'text-red-700';
      icon = '🔴';
    } else if (diffInMinutes < 60) {
      type = 'today';
      text = i18n.language === 'lo' ? `${diffInMinutes}ນາທີກ່ອນ` : `${diffInMinutes}m`;
      bgColor = 'bg-orange-100';
      textColor = 'text-orange-700';
      icon = '⏰';
    } else if (diffInHours < 24) {
      type = 'today';
      text = i18n.language === 'lo' ? `${diffInHours}ຊົ່ວໂມງກ່ອນ` : `${diffInHours}h`;
      bgColor = 'bg-blue-100';
      textColor = 'text-blue-700';
      icon = '📅';
    } else if (diffInDays < 7) {
      type = 'recent';
      text = i18n.language === 'lo' ? `${diffInDays}ມື້ທີ່ເເລ້ວ` : `${diffInDays}d`;
      bgColor = 'bg-purple-100';
      textColor = 'text-purple-700';
      icon = '📆';
    } else {
      type = 'older';
      text = date.toLocaleDateString(i18n.language, { 
        month: 'short', 
        day: 'numeric' 
      });
      bgColor = 'bg-gray-100';
      textColor = 'text-gray-700';
      icon = '📅';
    }

    return { type, text, bgColor, textColor, icon, fullDate: date };
  };

  const { type, text, bgColor, textColor, icon, fullDate } = getTimeInfo(dateString);

  const getBadgeStyles = () => {
    const baseStyles = `inline-flex items-center px-3 py-1.5 rounded text-xs font-semibold ${textColor} ${bgColor} border border-gray-200 transition-all duration-200 hover:border-gray-300`;
    
    switch (type) {
      case 'live':
        return `${baseStyles} animate-pulse`;
      case 'today':
        return baseStyles;
      case 'recent':
        return baseStyles;
      case 'older':
        return baseStyles;
      default:
        return baseStyles;
    }
  };

  return (
    <div className={`date-badge relative ${className}`}>
      <div className={getBadgeStyles()}>
        <span>{text}</span>
      </div>
      
      {/* Tooltip on hover */}
      <div className="absolute bottom-full left-1/2 transform -translate-x-1/2 mb-2 px-3 py-2 bg-gray-900 text-white text-xs rounded-lg opacity-0 group-hover:opacity-100 transition-opacity duration-200 pointer-events-none whitespace-nowrap z-50">
        {fullDate.toLocaleString(i18n.language, {
          weekday: 'long',
          year: 'numeric',
          month: 'long',
          day: 'numeric',
          hour: '2-digit',
          minute: '2-digit'
        })}
        <div className="absolute top-full left-1/2 transform -translate-x-1/2 -mt-1">
          <div className="border-4 border-transparent border-t-gray-900"></div>
        </div>
      </div>
    </div>
  );
}
